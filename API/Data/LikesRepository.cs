using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers.Pagination;
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

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var likes = _context.Likes.AsQueryable();

            var users = likesParams.Predicate switch
            {
                "likedBy" => likes.Where(like => like.LikedUserId == likesParams.UserId).Select(like => like.SourceUser),
                _ => likes.Where(like => like.SourceUserId == likesParams.UserId).Select(like => like.LikedUser)
            };

            var likeDtos = users.Select(user => new LikeDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likeDtos, likesParams.CurrentPage, likesParams.ItemsPerPage);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        => await _context.Users.Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}