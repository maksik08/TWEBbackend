using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class RemoteDiagnosticSessionDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SupportTicketId { get; set; }

        [ForeignKey(nameof(SupportTicketId))]
        public SupportTicketDomain? SupportTicket { get; set; }

        public int AgentId { get; set; }

        [ForeignKey(nameof(AgentId))]
        public UserDomain? Agent { get; set; }

        public RemoteDiagnosticStatus Status { get; set; } = RemoteDiagnosticStatus.Scheduled;

        public DateTime? ScheduledAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(2000)]
        public string? Result { get; set; }
    }
}
