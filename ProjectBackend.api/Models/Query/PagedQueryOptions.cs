namespace ProjectBackend.api.Models.Query
{
    public abstract class PagedQueryOptions
    {
        public int Page { get; init; }

        public int PageSize { get; init; }

        public string SortBy { get; init; } = string.Empty;

        public bool SortDescending { get; init; }

        public int Skip => (Page - 1) * PageSize;
    }
}
