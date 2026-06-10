using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class WarehouseDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(120)]
        public required string Name { get; set; }

        [MaxLength(40)]
        public required string Code { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<WarehouseZoneDomain> Zones { get; set; } = new List<WarehouseZoneDomain>();
    }
}
