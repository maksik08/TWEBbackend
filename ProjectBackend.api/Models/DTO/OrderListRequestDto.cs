using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.DTO
{
    public class OrderListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }
        public OrderStatus? Status { get; set; }
        public int? UserId { get; set; }
        public int? CustomerId { get; set; }
    }
}
