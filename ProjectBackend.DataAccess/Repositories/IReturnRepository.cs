using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IReturnRepository
    {
        Task<ReturnDomain> CreateAsync(ReturnDomain entity, CancellationToken cancellationToken);
        Task<PagedResult<ReturnDomain>> GetAllAsync(ReturnQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<ReturnDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ReturnDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken);
        Task<ReturnDomain> UpdateAsync(ReturnDomain entity, CancellationToken cancellationToken);
        Task<bool> HasActiveForOrderAsync(int orderId, CancellationToken cancellationToken);
    }
}
