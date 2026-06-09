using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class CreateServiceRequestDto
    {
        [Range(1, int.MaxValue)]
        public int ServiceTariffId { get; set; }

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
