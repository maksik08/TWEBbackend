using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.DTO
{
    public class ServiceRequestStatusReportItemDto
    {
        public ServiceRequestStatus Status { get; set; }

        public int Count { get; set; }
    }
}
