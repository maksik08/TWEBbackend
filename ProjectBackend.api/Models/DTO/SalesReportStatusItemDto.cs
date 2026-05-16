using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.DTO
{
    public class SalesReportStatusItemDto
    {
        public OrderStatus Status { get; set; }

        public int Count { get; set; }
    }
}
