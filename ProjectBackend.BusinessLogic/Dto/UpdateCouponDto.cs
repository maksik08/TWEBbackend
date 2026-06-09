using System.ComponentModel.DataAnnotations;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class UpdateCouponDto
    {
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(DiscountType))]
        public DiscountType DiscountType { get; set; }

        [Range(0.01, 1000000)]
        public decimal DiscountValue { get; set; }

        [Range(0, 1000000)]
        public decimal MinOrderAmount { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxUses { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
