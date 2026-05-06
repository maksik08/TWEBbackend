namespace ProjectBackend.api.Models.DTO
{
    public class PaginationMetadataDto
    {
        public int Page { get; init; }

        public int PageSize { get; init; }

        public int TotalCount { get; init; }

        public int TotalPages { get; init; }

        public bool HasNextPage { get; init; }

        public bool HasPreviousPage { get; init; }
    }
}
