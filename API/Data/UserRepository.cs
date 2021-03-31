using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using API.Helpers.Pagination;
using System;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(user => user.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
                .AsQueryable()
                .Where(u => u.UserName != userParams.CurrentUsername)
                .Where(u => u.Gender == userParams.Gender)
                .Where(u => u.DateOfBirth <= DateTime.Today.AddYears(-userParams.MinAge))
                .Where(u => userParams.MaxAge.HasValue ? u.DateOfBirth >= DateTime.Today.AddYears(-userParams.MaxAge.Value - 1) : true)
                .AsNoTracking();

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive),
            };

            return await PagedList<MemberDto>.CreateAsync(query
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider), userParams.CurrentPage, userParams.ItemsPerPage);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<string> GetUserGenderAsync(string username)
        => await _context.Users.Where(x => x.UserName == username).Select(x => x.Gender).FirstOrDefaultAsync();

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Photos)
                .ToListAsync();
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}