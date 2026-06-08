using ProjectBackend.Domain.Entities;

namespace ProjectBackend.Domain.Query
{
    public sealed class ContactMessageQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }

        public ContactMessageStatus? Status { get; init; }
    }
}
