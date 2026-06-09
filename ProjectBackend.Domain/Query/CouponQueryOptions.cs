namespace ProjectBackend.Domain.Query
{
    public sealed class CouponQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }

        public bool? IsActive { get; init; }
    }
}
