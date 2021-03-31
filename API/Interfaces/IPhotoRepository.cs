using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoDto>> GetPhotosToApprove();
        Task<Photo> ApprovePhotoAsync(int photoId);
        Task SetPhotoAsMain(Photo photo);
    }
}