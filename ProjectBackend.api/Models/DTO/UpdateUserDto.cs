using System.ComponentModel.DataAnnotations;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.DTO
{
    public class UpdateUserDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [MinLength(6)]
        public string? Password { get; set; }

        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; } = UserRole.User;
    }
}
