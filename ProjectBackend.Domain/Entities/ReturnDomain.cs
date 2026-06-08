using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class ReturnDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public OrderDomain? Order { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserDomain? User { get; set; }

        [MaxLength(1000)]
        public required string Reason { get; set; }

        public ReturnStatus Status { get; set; } = ReturnStatus.Requested;

        /// <summary>
        /// Refund amount captured when the return is created (order goods + services total).
        /// </summary>
        public decimal Amount { get; set; }

        [MaxLength(1000)]
        public string? Resolution { get; set; }

        public int? ProcessedByUserId { get; set; }

        public DateTime? ResolvedAt { get; set; }
    }
}
