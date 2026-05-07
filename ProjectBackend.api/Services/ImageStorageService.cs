using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ProjectBackend.api.Exceptions;

namespace ProjectBackend.api.Services
{
    public sealed class ImageStorageService : IImageStorageService
    {
        private const long MaxFileSize = 5 * 1024 * 1024;
        private const string ProductFolderName = "products";
        private const string UploadsRootName = "uploads";

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".gif",
        };

        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/gif",
        };

        private readonly IWebHostEnvironment _environment;

        public ImageStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveProductImageAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                throw new ValidationException("Image file is required.");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ValidationException("Image file is too large. Maximum size is 5 MB.");
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                throw new ValidationException("Unsupported image format. Allowed: jpg, jpeg, png, webp, gif.");
            }

            if (!string.IsNullOrWhiteSpace(file.ContentType) && !AllowedContentTypes.Contains(file.ContentType))
            {
                throw new ValidationException("Unsupported image content type.");
            }

            var folderPath = GetProductsFolderPath();
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var fullPath = Path.Combine(folderPath, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return $"/{UploadsRootName}/{ProductFolderName}/{fileName}";
        }

        public bool TryDeleteProductImage(string? relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl))
            {
                return false;
            }

            var expectedPrefix = $"/{UploadsRootName}/{ProductFolderName}/";
            if (!relativeUrl.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var fileName = Path.GetFileName(relativeUrl);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            var folderPath = GetProductsFolderPath();
            var fullPath = Path.GetFullPath(Path.Combine(folderPath, fileName));

            if (!fullPath.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!File.Exists(fullPath))
            {
                return false;
            }

            File.Delete(fullPath);
            return true;
        }

        private string GetProductsFolderPath()
        {
            var webRoot = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var folder = Path.Combine(webRoot, UploadsRootName, ProductFolderName);
            return Path.GetFullPath(folder);
        }
    }
}
