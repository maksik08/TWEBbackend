using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
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
            var query = BuildOrderQuery(tracked: false);

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(order =>
                    (order.User != null && order.User.Username.Contains(queryOptions.Search)) ||
                    (order.RecipientName != null && order.RecipientName.Contains(queryOptions.Search)) ||
                    (order.Phone != null && order.Phone.Contains(queryOptions.Search)) ||
                    (order.ShippingAddress != null && order.ShippingAddress.Contains(queryOptions.Search)) ||
                    (order.City != null && order.City.Contains(queryOptions.Search)));
            }

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(order => order.Status == queryOptions.Status.Value);
            }

            if (queryOptions.UserId.HasValue)
            {
                query = query.Where(order => order.UserId == queryOptions.UserId.Value);
            }

            query = queryOptions.SortBy switch
            {
                "subtotal" => queryOptions.SortDescending
                    ? query.OrderByDescending(order => order.Subtotal).ThenByDescending(order => order.Id)
                    : query.OrderBy(order => order.Subtotal).ThenBy(order => order.Id),
                "status" => queryOptions.SortDescending
                    ? query.OrderByDescending(order => order.Status).ThenByDescending(order => order.Id)
                    : query.OrderBy(order => order.Status).ThenBy(order => order.Id),
                _ => queryOptions.SortDescending
                    ? query.OrderByDescending(order => order.CreatedAt).ThenByDescending(order => order.Id)
                    : query.OrderBy(order => order.CreatedAt).ThenBy(order => order.Id)
            };

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<OrderDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await BuildOrderQuery(tracked: false)
                .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
        }

        public async Task<OrderDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await BuildOrderQuery(tracked: true)
                .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
        }

        public async Task<OrderDomain> CreateAsync(OrderDomain order, CancellationToken cancellationToken)
        {
            await _dbContext.Orders.AddAsync(order, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return await GetTrackedByIdAsync(order.Id, cancellationToken) ?? order;
        }

        public async Task<OrderDomain> UpdateAsync(OrderDomain order, CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return await GetTrackedByIdAsync(order.Id, cancellationToken) ?? order;
        }

        public async Task<IReadOnlyCollection<OrderDomain>> GetAllForReportAsync(CancellationToken cancellationToken)
        {
            return await BuildOrderQuery(tracked: false)
                .OrderByDescending(order => order.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        private IQueryable<OrderDomain> BuildOrderQuery(bool tracked)
        {
            var query = tracked
                ? _dbContext.Orders.AsQueryable()
                : _dbContext.Orders.AsNoTracking();

            return query
                .Include(order => order.User)
                .Include(order => order.Items);
        }
    }
}
