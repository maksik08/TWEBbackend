using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class GoodsReceiptDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(40)]
        public required string ReceiptNumber { get; set; }

        public int? PurchaseOrderId { get; set; }

        [ForeignKey(nameof(PurchaseOrderId))]
        public PurchaseOrderDomain? PurchaseOrder { get; set; }

        public int? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public SupplierDomain? Supplier { get; set; }

        public WarehouseReceiptStatus Status { get; set; } = WarehouseReceiptStatus.Draft;

        public bool QualityCheckPassed { get; set; }

        public bool CompletenessCheckPassed { get; set; }

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public ICollection<GoodsReceiptItemDomain> Items { get; set; } = new List<GoodsReceiptItemDomain>();
    }
}
