using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class AddServiceRequestCommentDto
    {
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}
