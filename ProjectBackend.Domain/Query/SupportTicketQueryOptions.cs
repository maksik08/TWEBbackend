using ProjectBackend.Domain.Entities;

namespace ProjectBackend.Domain.Query
{
    public sealed class SupportTicketQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }

        public SupportTicketStatus? Status { get; init; }

        public int? CustomerId { get; init; }

        public int? AssignedAgentId { get; init; }
    }
}
