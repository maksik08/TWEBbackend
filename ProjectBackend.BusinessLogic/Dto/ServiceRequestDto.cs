using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ServiceRequestDto : AuditableDto
    {
        public int Id { get; set; }

        public string RequestNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public string CustomerUsername { get; set; } = string.Empty;

        public int? InstallerId { get; set; }

        public string? InstallerUsername { get; set; }

        public int? ManagerId { get; set; }

        public string? ManagerUsername { get; set; }

        public string ServiceTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Address { get; set; } = string.Empty;

        public string ContactPhone { get; set; } = string.Empty;

        public DateTime? PreferredVisitAt { get; set; }

        public DateTime? ScheduledVisitAt { get; set; }

        public ServiceRequestStatus Status { get; set; }

        public string? CompletionReport { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ICollection<ServiceRequestCommentDto> Comments { get; set; } = new List<ServiceRequestCommentDto>();

        public ICollection<WorkPhotoDto> WorkPhotos { get; set; } = new List<WorkPhotoDto>();
    }
}
