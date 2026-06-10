using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class WarrantyClaimDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public UserDomain? Customer { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductsDomain? Product { get; set; }

        public int? OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public OrderDomain? Order { get; set; }

        public int? SupportTicketId { get; set; }

        [ForeignKey(nameof(SupportTicketId))]
        public SupportTicketDomain? SupportTicket { get; set; }

        public WarrantyClaimStatus Status { get; set; } = WarrantyClaimStatus.Open;

        public bool WarrantyValid { get; set; }

        public DateTime? PurchasedAt { get; set; }

        public DateTime? WarrantyExpiresAt { get; set; }

        [MaxLength(1000)]
        public required string IssueDescription { get; set; }

        [MaxLength(1000)]
        public string? Resolution { get; set; }
    }
}
