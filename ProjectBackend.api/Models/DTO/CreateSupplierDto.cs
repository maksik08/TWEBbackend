using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class CreateSupplierDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(200)]
        public string? ContactEmail { get; set; }

        [Phone]
        [MaxLength(50)]
        public string? Phone { get; set; }
    }
}
