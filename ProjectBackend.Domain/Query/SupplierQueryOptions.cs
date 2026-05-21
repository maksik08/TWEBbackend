namespace ProjectBackend.Domain.Query
{
    public sealed class SupplierQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }
    }
}
