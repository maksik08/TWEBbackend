using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public class ReturnRepository : IReturnRepository
    {
        private readonly ProjectDbContext _dbContext;

        public ReturnRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ReturnDomain> CreateAsync(ReturnDomain entity, CancellationToken cancellationToken)
        {
            await _dbContext.Returns.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<PagedResult<ReturnDomain>> GetAllAsync(ReturnQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.Returns
                .AsNoTracking()
                .Include(r => r.User)
                .AsQueryable();

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(r => r.Status == queryOptions.Status.Value);
            }

            if (queryOptions.OrderId.HasValue)
            {
                query = query.Where(r => r.OrderId == queryOptions.OrderId.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                var search = queryOptions.Search;
                query = query.Where(r =>
                    r.Reason.Contains(search) ||
                    (r.User != null && r.User.Username.Contains(search)));
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(r => r.CreatedAt).ThenByDescending(r => r.Id)
                : query.OrderBy(r => r.CreatedAt).ThenBy(r => r.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<ReturnDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Returns
                .AsNoTracking()
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<ReturnDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Returns
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<ReturnDomain> UpdateAsync(ReturnDomain entity, CancellationToken cancellationToken)
        {
            _dbContext.Returns.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public Task<bool> HasActiveForOrderAsync(int orderId, CancellationToken cancellationToken)
        {
            return _dbContext.Returns.AnyAsync(
                r => r.OrderId == orderId &&
                    (r.Status == ReturnStatus.Requested || r.Status == ReturnStatus.Approved),
                cancellationToken);
        }
    }
}
