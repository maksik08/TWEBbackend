using Microsoft.EntityFrameworkCore.Storage;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IOrderRepository
    {
        Task<PagedResult<OrderDomain>> GetAllAsync(OrderQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<OrderDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<OrderDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken);
        Task<OrderDomain> CreateAsync(OrderDomain order, CancellationToken cancellationToken);
        Task<OrderDomain> UpdateAsync(OrderDomain order, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<OrderDomain>> GetAllForReportAsync(CancellationToken cancellationToken);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
