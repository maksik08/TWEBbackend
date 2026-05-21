using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface ISupplierRepository
    {
        Task<PagedResult<SupplierDomain>> GetAllAsync(SupplierQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<SupplierDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsByNameAsync(string name, int? excludedId, CancellationToken cancellationToken);
        Task<bool> ExistsByContactEmailAsync(string email, int? excludedId, CancellationToken cancellationToken);
        Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken);
        Task<SupplierDomain> CreateAsync(SupplierDomain supplier, CancellationToken cancellationToken);
        Task<SupplierDomain?> UpdateAsync(int id, SupplierDomain supplier, CancellationToken cancellationToken);
        Task<SupplierDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
