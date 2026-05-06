using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.Query
{
    public sealed class OrderQueryOptions : PagedQueryOptions
    {
        public OrderStatus? Status { get; init; }
        public int? UserId { get; init; }
    }
}
