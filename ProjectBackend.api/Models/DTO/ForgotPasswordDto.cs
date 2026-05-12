using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
