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
        Task<OrderDomain> CreateAsync(OrderDomain order, CancellationToken cancellationToken);
        Task<OrderDomain?> UpdateStatusAsync(int id, OrderStatus status, DateTime? paidAt, CancellationToken cancellationToken);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
