using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Common;

namespace ProjectBackend.BusinessLogic.Services
{
    public class LocalAttachmentStorageService : IAttachmentStorageService
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".pdf",
            ".txt"
        };

        private readonly IWebHostEnvironment _environment;

        public LocalAttachmentStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<StoredWorkPhotoResult> SaveAsync(IFormFile file, string folder, CancellationToken cancellationToken)
        {
            if (file.Length <= 0)
            {
                throw new ValidationException("Uploaded file cannot be empty.");
            }

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ValidationException("Only JPG, PNG, WEBP, PDF and TXT files are supported.");
            }

            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var safeFolder = string.Join("-", folder.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
            var dateFolder = DateTime.UtcNow.ToString("yyyyMMdd");
            var directoryPath = Path.Combine(webRootPath, "uploads", safeFolder, dateFolder);
            Directory.CreateDirectory(directoryPath);

            var storedFileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(directoryPath, storedFileName);

            await using var stream = File.Create(filePath);
            await file.CopyToAsync(stream, cancellationToken);

            return new StoredWorkPhotoResult(file.FileName, $"/uploads/{safeFolder}/{dateFolder}/{storedFileName}");
        }
    }
}
