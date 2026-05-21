using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
