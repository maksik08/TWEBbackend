using AutoMapper;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class CouponService : ApplicationServiceBase, ICouponService
    {
        private readonly ICouponRepository _repository;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<CouponDto>> GetAllAsync(CouponListRequestDto request, CancellationToken cancellationToken)
        {
            var queryOptions = new CouponQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection ?? "desc"),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                IsActive = request.IsActive
            };

            var coupons = await _repository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<CouponDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<CouponDto>>(coupons.Items),
                TotalCount = coupons.TotalCount,
                Page = coupons.Page,
                PageSize = coupons.PageSize
            };
        }

        public async Task<CouponDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var coupon = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Coupon", id);
            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<CouponDto> CreateAsync(CreateCouponDto dto, CancellationToken cancellationToken)
        {
            var code = NormalizeCode(dto.Code);
            ValidateDiscount(dto.DiscountType, dto.DiscountValue);

            if (await _repository.ExistsByCodeAsync(code, null, cancellationToken))
            {
                throw new ConflictException("A coupon with this code already exists.");
            }

            var entity = new CouponDomain
            {
                Code = code,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinOrderAmount = dto.MinOrderAmount,
                MaxUses = dto.MaxUses,
                ExpiresAt = dto.ExpiresAt,
                IsActive = dto.IsActive
            };

            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<CouponDto>(created);
        }

        public async Task<CouponDto> UpdateAsync(int id, UpdateCouponDto dto, CancellationToken cancellationToken)
        {
            EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Coupon", id);

            var code = NormalizeCode(dto.Code);
            ValidateDiscount(dto.DiscountType, dto.DiscountValue);

            if (await _repository.ExistsByCodeAsync(code, id, cancellationToken))
            {
                throw new ConflictException("A coupon with this code already exists.");
            }

            var entity = new CouponDomain
            {
                Id = id,
                Code = code,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinOrderAmount = dto.MinOrderAmount,
                MaxUses = dto.MaxUses,
                ExpiresAt = dto.ExpiresAt,
                IsActive = dto.IsActive
            };

            var updated = EnsureFound(await _repository.UpdateAsync(entity, cancellationToken), "Coupon", id);
            return _mapper.Map<CouponDto>(updated);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            EnsureFound(await _repository.DeleteAsync(id, cancellationToken), "Coupon", id);
        }

        public async Task<CouponPreviewResultDto> PreviewAsync(PreviewCouponDto dto, CancellationToken cancellationToken)
        {
            var code = NormalizeCode(dto.Code);
            var coupon = await _repository.GetByCodeAsync(code, cancellationToken);
            if (coupon is null)
            {
                throw new NotFoundException("Promo code not found.");
            }

            CouponCalculator.EnsureApplicable(coupon, dto.Subtotal);
            var discount = CouponCalculator.ComputeDiscount(coupon, dto.Subtotal);

            return new CouponPreviewResultDto
            {
                Code = coupon.Code,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                Discount = discount
            };
        }

        private static string NormalizeCode(string code)
        {
            var normalized = code?.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new ValidationException("Coupon code is required.");
            }

            return normalized;
        }

        private static void ValidateDiscount(DiscountType type, decimal value)
        {
            if (value <= 0m)
            {
                throw new ValidationException("Discount value must be greater than zero.");
            }

            if (type == DiscountType.Percentage && value > 100m)
            {
                throw new ValidationException("Percentage discount cannot exceed 100.");
            }
        }
    }
}
