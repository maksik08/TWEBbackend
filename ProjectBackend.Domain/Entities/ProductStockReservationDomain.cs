using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class ProductStockReservationDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductsDomain? Product { get; set; }

        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public OrderDomain? Order { get; set; }

        public int Quantity { get; set; }

        public ProductStockReservationStatus Status { get; set; } = ProductStockReservationStatus.Reserved;

        public DateTime? ReleasedAt { get; set; }
    }
}
