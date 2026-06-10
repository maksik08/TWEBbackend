using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class StockMovementDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductsDomain? Product { get; set; }

        public int? WarehouseZoneId { get; set; }

        [ForeignKey(nameof(WarehouseZoneId))]
        public WarehouseZoneDomain? WarehouseZone { get; set; }

        public WarehouseMovementType Type { get; set; }

        public int QuantityDelta { get; set; }

        public int BalanceAfter { get; set; }

        [MaxLength(100)]
        public string? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
