using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.api.Models.Domain
{
    public class ServiceRequestDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(32)]
        public required string RequestNumber { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public UserDomain? Customer { get; set; }

        public int? InstallerId { get; set; }

        [ForeignKey(nameof(InstallerId))]
        public UserDomain? Installer { get; set; }

        public int? ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]
        public UserDomain? Manager { get; set; }

        [MaxLength(200)]
        public required string ServiceTitle { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(400)]
        public required string Address { get; set; }

        [MaxLength(50)]
        public required string ContactPhone { get; set; }

        public DateTime? PreferredVisitAt { get; set; }

        public DateTime? ScheduledVisitAt { get; set; }

        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Submitted;

        [MaxLength(2000)]
        public string? CompletionReport { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ICollection<ServiceRequestCommentDomain> Comments { get; set; } = new List<ServiceRequestCommentDomain>();

        public ICollection<WorkPhotoDomain> WorkPhotos { get; set; } = new List<WorkPhotoDomain>();
    }
}
