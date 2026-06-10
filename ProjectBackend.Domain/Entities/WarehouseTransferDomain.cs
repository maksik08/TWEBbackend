using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class WarehouseTransferDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductsDomain? Product { get; set; }

        public int? FromZoneId { get; set; }

        [ForeignKey(nameof(FromZoneId))]
        public WarehouseZoneDomain? FromZone { get; set; }

        public int ToZoneId { get; set; }

        [ForeignKey(nameof(ToZoneId))]
        public WarehouseZoneDomain? ToZone { get; set; }

        public int Quantity { get; set; }

        public WarehouseTransferStatus Status { get; set; } = WarehouseTransferStatus.Planned;

        public DateTime? CompletedAt { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
