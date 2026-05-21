using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IServiceRequestRepository
    {
        Task<PagedResult<ServiceRequestDomain>> GetAllAsync(ServiceRequestQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<ServiceRequestDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ServiceRequestDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken);
        Task<ServiceRequestDomain> CreateAsync(ServiceRequestDomain serviceRequest, CancellationToken cancellationToken);
        Task<ServiceRequestDomain> UpdateAsync(ServiceRequestDomain serviceRequest, CancellationToken cancellationToken);
        Task<ServiceRequestDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<ServiceRequestDomain>> GetAllForReportAsync(CancellationToken cancellationToken);
    }
}
