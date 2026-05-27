using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IProductReviewService
    {
        Task<IReadOnlyCollection<ProductReviewDto>> ListByProductAsync(int productId, CancellationToken cancellationToken);
        Task<ProductReviewDto> CreateAsync(int productId, CreateProductReviewDto dto, CancellationToken cancellationToken);
        Task DeleteAsync(int reviewId, CancellationToken cancellationToken);
    }
}
