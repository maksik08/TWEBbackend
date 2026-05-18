using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.api.Models.Domain
{
    public class PaymentTransactionDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserDomain? User { get; set; }

        public int? OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public OrderDomain? Order { get; set; }

        public PaymentTransactionType Type { get; set; }

        public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Completed;

        public PaymentMethod Method { get; set; }

        public decimal Amount { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "MDL";

        [MaxLength(100)]
        public string? ExternalReference { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
