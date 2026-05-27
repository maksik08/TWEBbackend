using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ProductDto : AuditableDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string? Brand { get; set; }
        public string? Sku { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Warranty { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsPreorder { get; set; }
        public bool IsVisible { get; set; }
        public string Availability { get; set; } = "in-stock";
        public ProductAvailability AvailabilityState { get; set; } = ProductAvailability.InStock;
        public List<string> Technology { get; set; } = new();
        public List<string> KeyFeatures { get; set; } = new();
        public List<string> PackageContents { get; set; } = new();
        public List<ProductSpecificationDto> Specifications { get; set; } = new();
        public int? CategoryId { get; set; }
        public string? Category { get; set; }
        public int? SupplierId { get; set; }
        public string? Supplier { get; set; }
    }
}
