using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public interface ISupplierRepository
    {
        Task<PagedResult<SupplierDomain>> GetAllAsync(SupplierQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<SupplierDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken);
        Task<SupplierDomain> CreateAsync(SupplierDomain supplier, CancellationToken cancellationToken);
        Task<SupplierDomain?> UpdateAsync(int id, SupplierDomain supplier, CancellationToken cancellationToken);
        Task<SupplierDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
