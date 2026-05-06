namespace ProjectBackend.api.Models.DTO
{
    /// <summary>
    /// Base query parameters for paged list endpoints.
    /// Example: page=1&amp;pageSize=10&amp;sortBy=name&amp;sortDirection=asc.
    /// </summary>
    public abstract class ListQueryRequestDto
    {
        /// <summary>
        /// Requested page number, starting from 1.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of records per page.
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Field used for sorting.
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Sorting direction: asc or desc.
        /// </summary>
        public string? SortDirection { get; set; }
    }
}
