namespace ProjectBackend.api.Models.Query
{
    public sealed class ProductQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }

        public int? CategoryId { get; init; }

        public int? SupplierId { get; init; }

        public decimal? MinPrice { get; init; }

        public decimal? MaxPrice { get; init; }
    }
}
