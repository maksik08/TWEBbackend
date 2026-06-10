using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class InventoryCountDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(40)]
        public required string CountNumber { get; set; }

        public int? WarehouseZoneId { get; set; }

        [ForeignKey(nameof(WarehouseZoneId))]
        public WarehouseZoneDomain? WarehouseZone { get; set; }

        public DateTime CountedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Note { get; set; }

        public ICollection<InventoryCountItemDomain> Items { get; set; } = new List<InventoryCountItemDomain>();
    }
}
