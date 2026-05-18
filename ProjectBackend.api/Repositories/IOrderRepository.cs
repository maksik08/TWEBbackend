using Microsoft.EntityFrameworkCore.Storage;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
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
