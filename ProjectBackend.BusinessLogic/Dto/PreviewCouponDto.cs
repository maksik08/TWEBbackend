using System.ComponentModel.DataAnnotations;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class PreviewCouponDto
    {
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Code { get; set; } = string.Empty;

        [Range(0, 100000000)]
        public decimal Subtotal { get; set; }
    }

    public class CouponPreviewResultDto
    {
        public string Code { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Discount { get; set; }
    }
}
