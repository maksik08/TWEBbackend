using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface ICategoryRepository
    {
        Task<PagedResult<CategoryDomain>> GetAllAsync(CategoryQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<CategoryDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsByNameAsync(string name, int? excludedId, CancellationToken cancellationToken);
        Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken);
        Task<CategoryDomain> CreateAsync(CategoryDomain category, CancellationToken cancellationToken);
        Task<CategoryDomain?> UpdateAsync(int id, CategoryDomain category, CancellationToken cancellationToken);
        Task<CategoryDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
