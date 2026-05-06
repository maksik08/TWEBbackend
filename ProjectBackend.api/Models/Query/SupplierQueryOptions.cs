namespace ProjectBackend.api.Models.Query
{
    public sealed class SupplierQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }
    }
}
