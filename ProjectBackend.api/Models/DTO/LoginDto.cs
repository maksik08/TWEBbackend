using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class LoginDto
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
