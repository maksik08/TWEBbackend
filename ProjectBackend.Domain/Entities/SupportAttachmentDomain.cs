using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class SupportAttachmentDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TicketId { get; set; }

        [ForeignKey(nameof(TicketId))]
        public SupportTicketDomain? Ticket { get; set; }

        public int UploadedByUserId { get; set; }

        [ForeignKey(nameof(UploadedByUserId))]
        public UserDomain? UploadedByUser { get; set; }

        [MaxLength(260)]
        public required string FileName { get; set; }

        [MaxLength(500)]
        public required string FilePath { get; set; }
    }
}
