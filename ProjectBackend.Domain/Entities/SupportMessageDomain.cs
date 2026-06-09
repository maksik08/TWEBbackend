using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class SupportMessageDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TicketId { get; set; }

        [ForeignKey(nameof(TicketId))]
        public SupportTicketDomain? Ticket { get; set; }

        public int AuthorUserId { get; set; }

        [ForeignKey(nameof(AuthorUserId))]
        public UserDomain? AuthorUser { get; set; }

        [MaxLength(4000)]
        public required string Text { get; set; }
    }
}
