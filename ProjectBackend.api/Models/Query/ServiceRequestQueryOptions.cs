using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.Query
{
    public sealed class ServiceRequestQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }

        public ServiceRequestStatus? Status { get; init; }

        public int? CustomerId { get; init; }

        public int? InstallerId { get; init; }

        public DateTime? ScheduledFrom { get; init; }

        public DateTime? ScheduledTo { get; init; }
    }
}
