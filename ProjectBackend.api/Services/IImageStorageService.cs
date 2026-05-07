using Microsoft.AspNetCore.Http;

namespace ProjectBackend.api.Services
{
    public interface IImageStorageService
    {
        Task<string> SaveProductImageAsync(IFormFile file, CancellationToken cancellationToken);

        bool TryDeleteProductImage(string? relativeUrl);
    }
}
