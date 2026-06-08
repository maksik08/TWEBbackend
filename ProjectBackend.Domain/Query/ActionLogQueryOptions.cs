namespace ProjectBackend.Domain.Query
{
    public sealed class ActionLogQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }

        public string? EntityType { get; init; }

        public string? Action { get; init; }
    }
}
