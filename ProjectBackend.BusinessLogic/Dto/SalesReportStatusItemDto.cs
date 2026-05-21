using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class SalesReportStatusItemDto
    {
        public OrderStatus Status { get; set; }

        public int Count { get; set; }
    }
}
