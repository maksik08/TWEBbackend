namespace ProjectBackend.Domain.Query
{
    public sealed class CategoryQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }
    }
}
