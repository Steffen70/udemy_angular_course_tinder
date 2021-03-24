using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        => await _context.Likes.FindAsync(sourceUserId, likedUserId);

        public async Task<IEnumerable<LikeDto>> GetUserLikes(int userId, string predicate)
        {
            var likes = _context.Likes.AsQueryable();

            var users = predicate switch
            {
                "likedBy" => likes.Where(like => like.LikedUserId == userId).Select(like => like.SourceUser),
                _ => likes.Where(like => like.SourceUserId == userId).Select(like => like.LikedUser)
            };

            return await users.Select(user => new LikeDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        => await _context.Users.Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}