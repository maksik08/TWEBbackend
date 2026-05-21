using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface ICustomerRepository
    {
        Task<PagedResult<CustomerDomain>> GetAllAsync(CustomerQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<CustomerDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CustomerDomain> CreateAsync(CustomerDomain customer, CancellationToken cancellationToken);
        Task<CustomerDomain?> UpdateAsync(int id, CustomerDomain customer, CancellationToken cancellationToken);
        Task<CustomerDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
