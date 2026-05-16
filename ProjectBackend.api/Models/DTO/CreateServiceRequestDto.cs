using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class CreateServiceRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string ServiceTitle { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(400)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ContactPhone { get; set; } = string.Empty;

        public DateTime? PreferredVisitAt { get; set; }
    }
}
