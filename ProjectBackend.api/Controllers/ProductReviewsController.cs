using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Product review endpoints.
    /// Read access: guest.
    /// Write access: authenticated user (one review per product per user).
    /// Delete: review owner or admin.
    /// </summary>
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IProductReviewService _service;

        public ProductReviewsController(IProductReviewService service)
        {
            _service = service;
        }

        [HttpGet("api/products/{productId:int}/reviews")]
        public async Task<IActionResult> List([FromRoute] int productId, CancellationToken cancellationToken)
        {
            var reviews = await _service.ListByProductAsync(productId, cancellationToken);
            return Ok(ApiResponse<IReadOnlyCollection<ProductReviewDto>>.Ok(reviews));
        }

        [UserAccess]
        [HttpPost("api/products/{productId:int}/reviews")]
        public async Task<IActionResult> Create(
            [FromRoute] int productId,
            [FromBody] CreateProductReviewDto dto,
            CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(productId, dto, cancellationToken);
            return Ok(ApiResponse<ProductReviewDto>.Ok(created, "Review posted successfully."));
        }

        [UserAccess]
        [HttpDelete("api/reviews/{reviewId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int reviewId, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(reviewId, cancellationToken);
            return Ok(ApiResponse<object?>.Ok(null, "Review deleted successfully."));
        }
    }
}
