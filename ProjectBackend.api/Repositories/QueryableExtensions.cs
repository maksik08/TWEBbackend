using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            PagedQueryOptions queryOptions,
            CancellationToken cancellationToken)
        {
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip(queryOptions.Skip)
                .Take(queryOptions.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            };
        }
    }
}
