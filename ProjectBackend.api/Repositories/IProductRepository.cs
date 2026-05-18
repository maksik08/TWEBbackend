using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public interface IProductRepository
    {
        Task<PagedResult<ProductsDomain>> GetAllAsync(ProductQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<ProductsDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<ProductsDomain>> GetByIdsAsync(IReadOnlyCollection<int> ids, bool includeHidden, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<ProductsDomain>> GetTrackedByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<ProductsDomain>> GetAllForStockReportAsync(CancellationToken cancellationToken);
        Task<ProductsDomain> CreateAsync(ProductsDomain product, CancellationToken cancellationToken);
        Task<ProductsDomain?> UpdateAsync(int id, ProductsDomain product, CancellationToken cancellationToken);
        Task<ProductsDomain?> SetVisibilityAsync(int id, bool isVisible, CancellationToken cancellationToken);
        Task<ProductsDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
