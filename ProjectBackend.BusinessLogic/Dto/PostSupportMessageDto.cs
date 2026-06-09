using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class PostSupportMessageDto
    {
        [Required]
        [StringLength(4000, MinimumLength = 1)]
        public string Text { get; set; } = string.Empty;
    }
}
