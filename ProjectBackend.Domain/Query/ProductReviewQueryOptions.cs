using ProjectBackend.Domain.Entities;

namespace ProjectBackend.Domain.Query
{
    public sealed class ProductReviewQueryOptions : PagedQueryOptions
    {
        public int? ProductId { get; init; }
        public int? Rating { get; init; }
        public ProductReviewStatus? Status { get; init; }
        public string? Search { get; init; }
        public bool IncludeHidden { get; init; }
    }

    public sealed class ProductReviewReportQueryOptions : PagedQueryOptions
    {
        public ProductReviewReportStatus? Status { get; init; }
    }
}
