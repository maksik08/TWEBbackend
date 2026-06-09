using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Services
{
    /// <summary>
    /// Server-side promo code validation and discount computation.
    /// </summary>
    public static class CouponCalculator
    {
        /// <summary>
        /// Throws a ValidationException when the coupon cannot be applied to the given goods subtotal.
        /// </summary>
        public static void EnsureApplicable(CouponDomain coupon, decimal subtotal)
        {
            if (!coupon.IsActive)
            {
                throw new ValidationException("This promo code is not active.");
            }

            if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < DateTime.UtcNow)
            {
                throw new ValidationException("This promo code has expired.");
            }

            if (coupon.MaxUses.HasValue && coupon.UsedCount >= coupon.MaxUses.Value)
            {
                throw new ValidationException("This promo code has reached its usage limit.");
            }

            if (subtotal < coupon.MinOrderAmount)
            {
                throw new ValidationException(
                    $"This promo code requires a minimum order of {coupon.MinOrderAmount:0.00}.");
            }
        }

        /// <summary>
        /// Computes the discount amount for a goods subtotal, never exceeding the subtotal.
        /// </summary>
        public static decimal ComputeDiscount(CouponDomain coupon, decimal subtotal)
        {
            var discount = coupon.DiscountType switch
            {
                DiscountType.Percentage => Math.Round(subtotal * (coupon.DiscountValue / 100m), 2, MidpointRounding.AwayFromZero),
                DiscountType.Fixed => coupon.DiscountValue,
                _ => 0m
            };

            if (discount < 0m)
            {
                discount = 0m;
            }

            return Math.Min(discount, subtotal);
        }
    }
}
