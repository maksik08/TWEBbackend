using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class SupportTicketDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(200)]
        public required string Subject { get; set; }

        public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;

        public SupportPriority Priority { get; set; } = SupportPriority.Normal;

        public DateTime? EscalatedAt { get; set; }

        public byte? SatisfactionRating { get; set; }

        [MaxLength(1000)]
        public string? SatisfactionComment { get; set; }

        /// <summary>The customer who opened the ticket.</summary>
        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public UserDomain? Customer { get; set; }

        /// <summary>The support agent assigned to the ticket, if any.</summary>
        public int? AssignedAgentId { get; set; }

        [ForeignKey(nameof(AssignedAgentId))]
        public UserDomain? AssignedAgent { get; set; }

        public ICollection<SupportMessageDomain> Messages { get; set; } = new List<SupportMessageDomain>();

        public ICollection<SupportAttachmentDomain> Attachments { get; set; } = new List<SupportAttachmentDomain>();
    }
}
