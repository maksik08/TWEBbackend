using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class ServiceTariffDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
