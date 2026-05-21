using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class OrderListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }
        public OrderStatus? Status { get; set; }
        public int? UserId { get; set; }
        public int? CustomerId { get; set; }
    }
}
