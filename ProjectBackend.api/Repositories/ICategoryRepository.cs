using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public interface ICategoryRepository
    {
        Task<PagedResult<CategoryDomain>> GetAllAsync(CategoryQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<CategoryDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken);
        Task<CategoryDomain> CreateAsync(CategoryDomain category, CancellationToken cancellationToken);
        Task<CategoryDomain?> UpdateAsync(int id, CategoryDomain category, CancellationToken cancellationToken);
        Task<CategoryDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
