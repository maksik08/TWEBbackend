using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class CreateContactMessageDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public required string Email { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public required string Subject { get; set; }

        [Required]
        [StringLength(4000, MinimumLength = 10)]
        public required string Message { get; set; }
    }
}
