using Microsoft.AspNetCore.Http;
using ProjectBackend.api.Models.Common;

namespace ProjectBackend.api.Services
{
    public interface IWorkPhotoStorageService
    {
        Task<IReadOnlyCollection<StoredWorkPhotoResult>> SaveAsync(ICollection<IFormFile> files, CancellationToken cancellationToken);
    }
}
