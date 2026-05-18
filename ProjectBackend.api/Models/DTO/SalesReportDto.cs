namespace ProjectBackend.api.Models.DTO
{
    public class SalesReportDto
    {
        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public ICollection<SalesReportStatusItemDto> Statuses { get; set; } = new List<SalesReportStatusItemDto>();
    }
}
