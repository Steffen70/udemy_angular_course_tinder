using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public PhotoRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<Photo> ApprovePhotoAsync(int photoId)
        {
            var currentPhoto = await _context.Photos.FindAsync(photoId);
            if (currentPhoto is not null)
                currentPhoto.IsApproved = true;

            return currentPhoto;
        }

        public async Task SetPhotoAsMain(Photo photo)
        {
            var count = await _context.Photos.CountAsync(p => p.AppUserId == photo.AppUserId);

            if (count <= 1)
                photo.IsMain = true;
        }

        public async Task<IEnumerable<PhotoDto>> GetPhotosToApprove()
        => await _context.Photos.Where(p => !p.IsApproved).ProjectTo<PhotoDto>(_mapper.ConfigurationProvider).ToListAsync();
    }
}