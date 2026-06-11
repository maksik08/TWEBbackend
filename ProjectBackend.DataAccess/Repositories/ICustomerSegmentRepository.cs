using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;
namespace ProjectBackend.DataAccess.Repositories
{
    public interface ICustomerSegmentRepository
    {
        Task<PagedResult<CustomerSegmentDomain>> GetAllAsync(PagedQueryOptions options, CancellationToken cancellationToken);
        Task<CustomerSegmentDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CustomerSegmentDomain> CreateAsync(CustomerSegmentDomain segment, CancellationToken cancellationToken);
        Task<CustomerSegmentDomain?> UpdateAsync(CustomerSegmentDomain segment, CancellationToken cancellationToken);
        Task<CustomerSegmentDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
        Task AssignCustomerAsync(int segmentId, int customerId, CancellationToken cancellationToken);
        Task RemoveAssignmentAsync(int segmentId, int customerId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<CustomerSegmentDomain>> GetSegmentsForCustomerAsync(int customerId, CancellationToken cancellationToken);
    }
}
