using Microsoft.AspNetCore.Http;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class UploadImageRequestDto
    {
        public IFormFile File { get; set; } = default!;
    }
}
