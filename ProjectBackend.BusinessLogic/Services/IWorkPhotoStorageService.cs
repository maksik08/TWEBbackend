using Microsoft.AspNetCore.Http;
using ProjectBackend.Domain.Common;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IWorkPhotoStorageService
    {
        Task<IReadOnlyCollection<StoredWorkPhotoResult>> SaveAsync(ICollection<IFormFile> files, CancellationToken cancellationToken);
    }
}
