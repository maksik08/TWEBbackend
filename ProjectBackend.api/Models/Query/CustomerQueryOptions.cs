namespace ProjectBackend.api.Models.Query
{
    public sealed class CustomerQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }
    }
}
