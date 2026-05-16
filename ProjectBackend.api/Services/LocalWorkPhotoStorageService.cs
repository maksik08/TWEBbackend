using Microsoft.AspNetCore.Http;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;

namespace ProjectBackend.api.Services
{
    public class LocalWorkPhotoStorageService : IWorkPhotoStorageService
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private readonly IWebHostEnvironment _environment;

        public LocalWorkPhotoStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<IReadOnlyCollection<StoredWorkPhotoResult>> SaveAsync(
            ICollection<IFormFile> files,
            CancellationToken cancellationToken)
        {
            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var dateFolder = DateTime.UtcNow.ToString("yyyyMMdd");
            var directoryPath = Path.Combine(webRootPath, "uploads", "work-photos", dateFolder);
            Directory.CreateDirectory(directoryPath);

            var storedFiles = new List<StoredWorkPhotoResult>();
            foreach (var file in files)
            {
                if (file.Length <= 0)
                {
                    throw new ValidationException("Uploaded photo cannot be empty.");
                }

                var extension = Path.GetExtension(file.FileName);
                if (!AllowedExtensions.Contains(extension))
                {
                    throw new ValidationException("Only JPG, PNG, and WEBP work photos are supported.");
                }

                var storedFileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(directoryPath, storedFileName);

                await using var stream = File.Create(filePath);
                await file.CopyToAsync(stream, cancellationToken);

                var relativePath = $"/uploads/work-photos/{dateFolder}/{storedFileName}";
                storedFiles.Add(new StoredWorkPhotoResult(file.FileName, relativePath));
            }

            return storedFiles;
        }
    }
}
