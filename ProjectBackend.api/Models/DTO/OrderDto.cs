using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.DTO
{
    public class OrderDto : AuditableDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? RecipientName { get; set; }
        public string? Phone { get; set; }
        public string? ShippingAddress { get; set; }
        public string? City { get; set; }
        public string? Comment { get; set; }
        public IReadOnlyCollection<OrderItemDto> Items { get; set; } = Array.Empty<OrderItemDto>();
    }
}
