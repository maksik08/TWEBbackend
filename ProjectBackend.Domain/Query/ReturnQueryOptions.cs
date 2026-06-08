using ProjectBackend.Domain.Entities;

namespace ProjectBackend.Domain.Query
{
    public sealed class ReturnQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }

        public ReturnStatus? Status { get; init; }

        public int? OrderId { get; init; }
    }
}
