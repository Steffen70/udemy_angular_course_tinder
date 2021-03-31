using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoRepository _photoRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _photoRepository = unitOfWork.PhotoRepository;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                }).ToListAsync();

            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles([FromRoute] string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user is null) return NotFound($"Could not find user with username: '{username}'");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));

        }

        [Authorize(Policy = "ModeratorPhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosForModeration()
        => Ok(await _photoRepository.GetPhotosToApprove());

        [Authorize(Policy = "ModeratorPhotoRole")]
        [HttpPut("photo-approve/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int? photoId)
        {
            var photo = photoId.HasValue ? await _photoRepository.ApprovePhotoAsync(photoId.Value) : null;

            if (photo is null)
                return BadRequest("This photo cannot be found");

            await _photoRepository.SetPhotoAsMain(photo);

            if (_unitOfWork.HasChanges() && await _unitOfWork.Complete())
                return Ok();

            if (photo is not null && photo.IsApproved)
                return BadRequest("This photo is already approved");

            return BadRequest($"The photo '{photoId.Value}' could not be approved");
        }
    }
}