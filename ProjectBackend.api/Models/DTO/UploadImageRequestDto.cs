using Microsoft.AspNetCore.Http;

namespace ProjectBackend.api.Models.DTO
{
    public class UploadImageRequestDto
    {
        public IFormFile File { get; set; } = default!;
    }
}
