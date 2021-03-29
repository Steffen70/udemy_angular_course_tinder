using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers.Pagination;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LikesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = unitOfWork.UserRepository;
            _likesRepository = unitOfWork.LikesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike([FromRoute] string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUserNameAsync(username);
            var SourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null)
                return NotFound();

            if (SourceUser.UserName == username)
                return BadRequest("You cannot like yourselft");

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null)
                return BadRequest("You already liked this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            SourceUser.LikedUsers.Add(userLike);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest($"Failed to like user '{username}'");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader<LikesHeader, LikeDto, LikesParams>(users, _mapper, likesParams);
            return Ok(users);
        }
    }
}