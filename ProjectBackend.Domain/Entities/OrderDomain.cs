using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class OrderDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserDomain? User { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal Subtotal { get; set; }

        public DateTime? PaidAt { get; set; }

        [MaxLength(120)]
        public string? RecipientName { get; set; }

        [MaxLength(40)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? ShippingAddress { get; set; }

        [MaxLength(80)]
        public string? City { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public ICollection<OrderItemDomain> Items { get; set; } = new List<OrderItemDomain>();

        public ICollection<PaymentTransactionDomain> PaymentTransactions { get; set; } = new List<PaymentTransactionDomain>();
    }
}
