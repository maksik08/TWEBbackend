using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class PurchaseOrderDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(40)]
        public required string OrderNumber { get; set; }

        public int SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public SupplierDomain? Supplier { get; set; }

        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

        public DateTime? ExpectedAt { get; set; }

        public DateTime? OrderedAt { get; set; }

        public DateTime? ReceivedAt { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public ICollection<PurchaseOrderItemDomain> Items { get; set; } = new List<PurchaseOrderItemDomain>();
    }
}
