using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IProductReviewRepository
    {
        Task<IReadOnlyCollection<ProductReviewDomain>> GetByProductIdAsync(int productId, CancellationToken cancellationToken);
        Task<PagedResult<ProductReviewDomain>> GetAllAsync(ProductReviewQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<ProductReviewDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ProductReviewDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsByUserAndProductAsync(int userId, int productId, CancellationToken cancellationToken);
        Task<ProductReviewDomain> CreateAsync(ProductReviewDomain review, CancellationToken cancellationToken);
        Task<ProductReviewPhotoDomain> AddPhotoAsync(ProductReviewPhotoDomain photo, CancellationToken cancellationToken);
        Task<ProductReviewReportDomain> AddReportAsync(ProductReviewReportDomain report, CancellationToken cancellationToken);
        Task<PagedResult<ProductReviewReportDomain>> GetReportsAsync(ProductReviewReportQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<ProductReviewReportDomain?> GetTrackedReportAsync(int id, CancellationToken cancellationToken);
        Task<ReviewResponseTemplateDomain> CreateTemplateAsync(ReviewResponseTemplateDomain template, CancellationToken cancellationToken);
        Task<PagedResult<ReviewResponseTemplateDomain>> GetTemplatesAsync(PagedQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<ReviewResponseTemplateDomain?> GetTrackedTemplateAsync(int id, CancellationToken cancellationToken);
        Task<ReviewResponseTemplateDomain> UpdateTemplateAsync(ReviewResponseTemplateDomain template, CancellationToken cancellationToken);
        Task DeleteAsync(ProductReviewDomain review, CancellationToken cancellationToken);
    }
}
