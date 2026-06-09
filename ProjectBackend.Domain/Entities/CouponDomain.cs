using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class CouponDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(40)]
        public required string Code { get; set; }

        public DiscountType DiscountType { get; set; } = DiscountType.Percentage;

        /// <summary>
        /// For Percentage: 0..100. For Fixed: an absolute amount in account currency.
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// Minimum order goods subtotal required to use the coupon. 0 means no minimum.
        /// </summary>
        public decimal MinOrderAmount { get; set; }

        /// <summary>
        /// Maximum number of times the coupon can be used overall. Null means unlimited.
        /// </summary>
        public int? MaxUses { get; set; }

        public int UsedCount { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
