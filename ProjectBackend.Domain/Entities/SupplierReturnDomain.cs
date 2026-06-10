using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class SupplierReturnDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(40)]
        public required string ReturnNumber { get; set; }

        public int SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public SupplierDomain? Supplier { get; set; }

        public SupplierReturnStatus Status { get; set; } = SupplierReturnStatus.Draft;

        [MaxLength(1000)]
        public required string Reason { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ICollection<SupplierReturnItemDomain> Items { get; set; } = new List<SupplierReturnItemDomain>();
    }
}
