using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface ICouponService
    {
        Task<PagedResult<CouponDto>> GetAllAsync(CouponListRequestDto request, CancellationToken cancellationToken);
        Task<CouponDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CouponDto> CreateAsync(CreateCouponDto dto, CancellationToken cancellationToken);
        Task<CouponDto> UpdateAsync(int id, UpdateCouponDto dto, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<CouponPreviewResultDto> PreviewAsync(PreviewCouponDto dto, CancellationToken cancellationToken);
    }
}
