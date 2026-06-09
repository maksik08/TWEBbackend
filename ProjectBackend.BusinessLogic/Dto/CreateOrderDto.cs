using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class CreateOrderDto
    {
        [Required]
        [MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; } = new();

        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [StringLength(40, MinimumLength = 5)]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(80, MinimumLength = 2)]
        public string City { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Comment { get; set; }

        /// <summary>
        /// Optional installation services. When present, their cost is computed server-side
        /// and added to the order total.
        /// </summary>
        public CreateOrderServicesDto? Services { get; set; }

        /// <summary>
        /// Optional promo code. Validated and applied server-side against the goods subtotal.
        /// </summary>
        [StringLength(40)]
        public string? CouponCode { get; set; }
    }
}
