using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ServiceRequestListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }

        public ServiceRequestStatus? Status { get; set; }

        public int? CustomerId { get; set; }

        public int? InstallerId { get; set; }

        public DateTime? ScheduledFrom { get; set; }

        public DateTime? ScheduledTo { get; set; }
    }
}
