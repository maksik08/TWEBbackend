using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class CreateCustomerDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(50)]
        public string? Phone { get; set; }
    }
}
