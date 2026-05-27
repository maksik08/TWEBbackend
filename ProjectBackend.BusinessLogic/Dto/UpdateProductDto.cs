using System.ComponentModel.DataAnnotations;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class UpdateProductDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }

        [MaxLength(100)]
        public string? Brand { get; set; }

        [MaxLength(64)]
        public string? Sku { get; set; }

        [MaxLength(500)]
        public string? ShortDescription { get; set; }

        [MaxLength(4000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Warranty { get; set; }

        public ProductAvailability Availability { get; set; } = ProductAvailability.InStock;

        public List<string> Technology { get; set; } = new();

        public List<string> KeyFeatures { get; set; } = new();

        public List<string> PackageContents { get; set; } = new();

        public List<ProductSpecificationDto> Specifications { get; set; } = new();

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public bool IsPreorder { get; set; }

        public bool IsVisible { get; set; } = true;

        public int? CategoryId { get; set; }

        public int? SupplierId { get; set; }
    }
}
