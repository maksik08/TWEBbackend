using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class AddServiceRequestCommentDto
    {
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}
