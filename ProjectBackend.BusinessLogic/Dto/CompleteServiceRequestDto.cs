using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class CompleteServiceRequestDto
    {
        [Required]
        [MaxLength(2000)]
        public string Report { get; set; } = string.Empty;

        public ICollection<IFormFile> Photos { get; set; } = new List<IFormFile>();
    }
}
