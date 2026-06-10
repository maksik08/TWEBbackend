using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IProductReviewService
    {
        Task<IReadOnlyCollection<ProductReviewDto>> ListByProductAsync(int productId, CancellationToken cancellationToken);
        Task<PagedResult<ProductReviewDto>> ListByProductAsync(int productId, ProductReviewListRequestDto request, CancellationToken cancellationToken);
        Task<PagedResult<ProductReviewDto>> GetModerationQueueAsync(ProductReviewListRequestDto request, CancellationToken cancellationToken);
        Task<ProductReviewDto> CreateAsync(int productId, CreateProductReviewDto dto, CancellationToken cancellationToken);
        Task<ProductReviewPhotoDto> AddPhotoAsync(int reviewId, UploadProductReviewPhotoDto dto, CancellationToken cancellationToken);
        Task<ProductReviewReportDto> ReportAsync(int reviewId, ReportProductReviewDto dto, CancellationToken cancellationToken);
        Task<PagedResult<ProductReviewReportDto>> GetReportsAsync(ProductReviewReportStatus? status, CancellationToken cancellationToken);
        Task<ProductReviewDto> UpdateStatusAsync(int reviewId, UpdateProductReviewStatusDto dto, CancellationToken cancellationToken);
        Task<ProductReviewDto> ReplyAsync(int reviewId, ReplyToProductReviewDto dto, CancellationToken cancellationToken);
        Task<PagedResult<ReviewResponseTemplateDto>> GetTemplatesAsync(ListQueryRequestDto request, CancellationToken cancellationToken);
        Task<ReviewResponseTemplateDto> CreateTemplateAsync(CreateReviewResponseTemplateDto dto, CancellationToken cancellationToken);
        Task<ReviewResponseTemplateDto> UpdateTemplateAsync(int id, UpdateReviewResponseTemplateDto dto, CancellationToken cancellationToken);
        Task DeleteAsync(int reviewId, CancellationToken cancellationToken);
    }
}
