using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class WorkPhotoDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ServiceRequestId { get; set; }

        [ForeignKey(nameof(ServiceRequestId))]
        public ServiceRequestDomain? ServiceRequest { get; set; }

        [MaxLength(260)]
        public required string FileName { get; set; }

        [MaxLength(500)]
        public required string FilePath { get; set; }
    }
}
