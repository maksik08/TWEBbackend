using ProjectBackend.api.Models.Common;

namespace ProjectBackend.api.Models.DTO
{
    public class PagedResponse<T> : ApiResponse<IReadOnlyCollection<T>>
    {
        public PaginationMetadataDto Pagination { get; init; } = new();

        public static PagedResponse<T> Ok(PagedResult<T> result, string message = "Request completed successfully.")
        {
            return new PagedResponse<T>
            {
                Success = true,
                Message = message,
                Data = result.Items,
                Pagination = new PaginationMetadataDto
                {
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalCount = result.TotalCount,
                    TotalPages = result.TotalPages,
                    HasNextPage = result.HasNextPage,
                    HasPreviousPage = result.HasPreviousPage
                }
            };
        }
    }
}
