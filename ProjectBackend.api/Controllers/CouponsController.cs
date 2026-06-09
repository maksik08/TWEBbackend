using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Promo code management (admin) and validation (authenticated users at checkout).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [AdminMod]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CouponListRequestDto request, CancellationToken cancellationToken)
        {
            var coupons = await _couponService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<CouponDto>.Ok(coupons));
        }

        [AdminMod]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var coupon = await _couponService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<CouponDto>.Ok(coupon));
        }

        [AdminMod]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCouponDto dto, CancellationToken cancellationToken)
        {
            var created = await _couponService.CreateAsync(dto, cancellationToken);
            return Ok(ApiResponse<CouponDto>.Ok(created, "Coupon created."));
        }

        [AdminMod]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCouponDto dto, CancellationToken cancellationToken)
        {
            var updated = await _couponService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<CouponDto>.Ok(updated, "Coupon updated."));
        }

        [AdminMod]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _couponService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse<object?>.Ok(null, "Coupon deleted."));
        }

        /// <summary>
        /// Validates a promo code against a goods subtotal and returns the resulting discount.
        /// Access: any authenticated user (used by the checkout page).
        /// </summary>
        [UserAccess]
        [HttpPost("preview")]
        public async Task<IActionResult> Preview([FromBody] PreviewCouponDto dto, CancellationToken cancellationToken)
        {
            var result = await _couponService.PreviewAsync(dto, cancellationToken);
            return Ok(ApiResponse<CouponPreviewResultDto>.Ok(result));
        }
    }
}
