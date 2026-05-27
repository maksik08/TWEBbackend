using Microsoft.EntityFrameworkCore;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly ProjectDbContext _dbContext;

        public ProductReviewRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<ProductReviewDomain>> GetByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            return await _dbContext.ProductReviews
                .AsNoTracking()
                .Include(review => review.User)
                .Where(review => review.ProductId == productId)
                .OrderByDescending(review => review.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductReviewDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.ProductReviews
                .Include(review => review.User)
                .FirstOrDefaultAsync(review => review.Id == id, cancellationToken);
        }

        public Task<bool> ExistsByUserAndProductAsync(int userId, int productId, CancellationToken cancellationToken)
        {
            return _dbContext.ProductReviews
                .AnyAsync(review => review.UserId == userId && review.ProductId == productId, cancellationToken);
        }

        public async Task<ProductReviewDomain> CreateAsync(ProductReviewDomain review, CancellationToken cancellationToken)
        {
            await _dbContext.ProductReviews.AddAsync(review, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _dbContext.Entry(review).Reference(r => r.User).LoadAsync(cancellationToken);
            return review;
        }

        public async Task DeleteAsync(ProductReviewDomain review, CancellationToken cancellationToken)
        {
            _dbContext.ProductReviews.Remove(review);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
