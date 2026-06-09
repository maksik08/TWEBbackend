using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class CreateSupportTicketDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(4000, MinimumLength = 1)]
        public string Message { get; set; } = string.Empty;
    }
}
