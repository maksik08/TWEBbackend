using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IProductReviewRepository
    {
        Task<IReadOnlyCollection<ProductReviewDomain>> GetByProductIdAsync(int productId, CancellationToken cancellationToken);
        Task<ProductReviewDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsByUserAndProductAsync(int userId, int productId, CancellationToken cancellationToken);
        Task<ProductReviewDomain> CreateAsync(ProductReviewDomain review, CancellationToken cancellationToken);
        Task DeleteAsync(ProductReviewDomain review, CancellationToken cancellationToken);
    }
}
