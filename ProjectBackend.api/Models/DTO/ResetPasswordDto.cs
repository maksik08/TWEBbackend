using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class ResetPasswordDto
    {
        [Required]
        public required string Token { get; set; }

        [Required]
        [MinLength(6)]
        public required string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
