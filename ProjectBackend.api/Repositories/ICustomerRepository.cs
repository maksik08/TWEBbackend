using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
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
