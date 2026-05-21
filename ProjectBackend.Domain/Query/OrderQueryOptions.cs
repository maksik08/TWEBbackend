using ProjectBackend.Domain.Entities;

namespace ProjectBackend.Domain.Query
{
    public sealed class OrderQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }
        public OrderStatus? Status { get; init; }
        public int? UserId { get; init; }
    }
}
