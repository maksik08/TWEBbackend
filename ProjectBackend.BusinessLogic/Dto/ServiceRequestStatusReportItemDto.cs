using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ServiceRequestStatusReportItemDto
    {
        public ServiceRequestStatus Status { get; set; }

        public int Count { get; set; }
    }
}
