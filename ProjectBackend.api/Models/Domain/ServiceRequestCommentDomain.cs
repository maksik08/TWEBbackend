using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.api.Models.Domain
{
    public class ServiceRequestCommentDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ServiceRequestId { get; set; }

        [ForeignKey(nameof(ServiceRequestId))]
        public ServiceRequestDomain? ServiceRequest { get; set; }

        public int AuthorUserId { get; set; }

        [ForeignKey(nameof(AuthorUserId))]
        public UserDomain? AuthorUser { get; set; }

        [MaxLength(1000)]
        public required string Message { get; set; }
    }
}
