using Microsoft.EntityFrameworkCore;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

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
                .Include(review => review.ModeratorUser)
                .Include(review => review.Photos)
                .Where(review => review.ProductId == productId)
                .Where(review => review.Status == ProductReviewStatus.Published)
                .OrderByDescending(review => review.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<ProductReviewDomain>> GetAllAsync(ProductReviewQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.ProductReviews
                .AsNoTracking()
                .Include(review => review.User)
                .Include(review => review.ModeratorUser)
                .Include(review => review.Photos)
                .AsQueryable();

            if (queryOptions.ProductId.HasValue)
            {
                query = query.Where(review => review.ProductId == queryOptions.ProductId.Value);
            }

            if (!queryOptions.IncludeHidden)
            {
                query = query.Where(review => review.Status == ProductReviewStatus.Published);
            }

            if (queryOptions.Rating.HasValue)
            {
                query = query.Where(review => review.Rating == queryOptions.Rating.Value);
            }

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(review => review.Status == queryOptions.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(review => review.Comment.Contains(queryOptions.Search));
            }

            query = queryOptions.SortBy == "rating"
                ? queryOptions.SortDescending
                    ? query.OrderByDescending(review => review.Rating).ThenByDescending(review => review.Id)
                    : query.OrderBy(review => review.Rating).ThenBy(review => review.Id)
                : queryOptions.SortDescending
                    ? query.OrderByDescending(review => review.CreatedAt).ThenByDescending(review => review.Id)
                    : query.OrderBy(review => review.CreatedAt).ThenBy(review => review.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<ProductReviewDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.ProductReviews
                .Include(review => review.User)
                .Include(review => review.ModeratorUser)
                .Include(review => review.Photos)
                .FirstOrDefaultAsync(review => review.Id == id, cancellationToken);
        }

        public async Task<ProductReviewDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.ProductReviews
                .Include(review => review.User)
                .Include(review => review.ModeratorUser)
                .Include(review => review.Photos)
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

        public async Task<ProductReviewPhotoDomain> AddPhotoAsync(ProductReviewPhotoDomain photo, CancellationToken cancellationToken)
        {
            await _dbContext.ProductReviewPhotos.AddAsync(photo, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return photo;
        }

        public async Task<ProductReviewReportDomain> AddReportAsync(ProductReviewReportDomain report, CancellationToken cancellationToken)
        {
            await _dbContext.ProductReviewReports.AddAsync(report, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(report).Reference(item => item.ReporterUser).LoadAsync(cancellationToken);
            await _dbContext.Entry(report).Reference(item => item.Review).LoadAsync(cancellationToken);
            return report;
        }

        public async Task<PagedResult<ProductReviewReportDomain>> GetReportsAsync(ProductReviewReportQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.ProductReviewReports
                .AsNoTracking()
                .Include(report => report.ReporterUser)
                .Include(report => report.Review)
                    .ThenInclude(review => review!.User)
                .AsQueryable();

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(report => report.Status == queryOptions.Status.Value);
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(report => report.CreatedAt).ThenByDescending(report => report.Id)
                : query.OrderBy(report => report.CreatedAt).ThenBy(report => report.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<ProductReviewReportDomain?> GetTrackedReportAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.ProductReviewReports
                .Include(report => report.Review)
                .FirstOrDefaultAsync(report => report.Id == id, cancellationToken);
        }

        public async Task<ReviewResponseTemplateDomain> CreateTemplateAsync(ReviewResponseTemplateDomain template, CancellationToken cancellationToken)
        {
            await _dbContext.ReviewResponseTemplates.AddAsync(template, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return template;
        }

        public async Task<PagedResult<ReviewResponseTemplateDomain>> GetTemplatesAsync(PagedQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.ReviewResponseTemplates
                .AsNoTracking()
                .OrderBy(template => template.Name)
                .AsQueryable();

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<ReviewResponseTemplateDomain?> GetTrackedTemplateAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.ReviewResponseTemplates.FirstOrDefaultAsync(template => template.Id == id, cancellationToken);
        }

        public async Task<ReviewResponseTemplateDomain> UpdateTemplateAsync(ReviewResponseTemplateDomain template, CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return template;
        }

        public async Task DeleteAsync(ProductReviewDomain review, CancellationToken cancellationToken)
        {
            _dbContext.ProductReviews.Remove(review);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
