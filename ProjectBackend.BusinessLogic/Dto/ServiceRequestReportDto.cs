namespace ProjectBackend.BusinessLogic.Dto
{
    public class ServiceRequestReportDto
    {
        public int TotalRequests { get; set; }

        public int AssignedRequests { get; set; }

        public int CompletedRequests { get; set; }

        public ICollection<ServiceRequestStatusReportItemDto> Statuses { get; set; } = new List<ServiceRequestStatusReportItemDto>();
    }
}
