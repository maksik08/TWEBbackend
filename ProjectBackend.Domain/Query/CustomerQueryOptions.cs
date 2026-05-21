namespace ProjectBackend.Domain.Query
{
    public sealed class CustomerQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }
    }
}
