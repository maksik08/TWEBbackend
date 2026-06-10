using ProjectBackend.Domain.Common;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IAttachmentStorageService
    {
        Task<StoredWorkPhotoResult> SaveAsync(IFormFile file, string folder, CancellationToken cancellationToken);
    }
}
