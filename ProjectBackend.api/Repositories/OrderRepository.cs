using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ProjectDbContext _dbContext;

        public OrderRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<OrderDomain>> GetAllAsync(OrderQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Items)
                .AsQueryable();

            if (queryOptions.UserId.HasValue)
            {
                query = query.Where(o => o.UserId == queryOptions.UserId.Value);
            }

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(o => o.Status == queryOptions.Status.Value);
            }

            query = queryOptions.SortBy switch
            {
                "subtotal" => queryOptions.SortDescending
                    ? query.OrderByDescending(o => o.Subtotal).ThenByDescending(o => o.Id)
                    : query.OrderBy(o => o.Subtotal).ThenBy(o => o.Id),
                "status" => queryOptions.SortDescending
                    ? query.OrderByDescending(o => o.Status).ThenByDescending(o => o.Id)
                    : query.OrderBy(o => o.Status).ThenBy(o => o.Id),
                _ => queryOptions.SortDescending
                    ? query.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id)
                    : query.OrderBy(o => o.CreatedAt).ThenBy(o => o.Id)
            };

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<OrderDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<OrderDomain> CreateAsync(OrderDomain order, CancellationToken cancellationToken)
        {
            await _dbContext.Orders.AddAsync(order, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return order;
        }

        public async Task<OrderDomain?> UpdateStatusAsync(int id, OrderStatus status, DateTime? paidAt, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Orders
                .Include(o => o.Items)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (existing is null) return null;

            existing.Status = status;
            if (paidAt.HasValue)
            {
                existing.PaidAt = paidAt.Value;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }
    }
}
