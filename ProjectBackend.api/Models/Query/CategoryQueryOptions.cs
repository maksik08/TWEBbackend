namespace ProjectBackend.api.Models.Query
{
    public sealed class CategoryQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }
    }
}
