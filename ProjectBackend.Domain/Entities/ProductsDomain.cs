using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class ProductsDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Name { get; set; }

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

        public List<ProductSpecification> Specifications { get; set; } = new();

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int ReservedQuantity { get; set; }

        public int MinStockLevel { get; set; }

        public int? MaxStockLevel { get; set; }

        public bool IsPreorder { get; set; }

        public bool IsVisible { get; set; } = true;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public CategoryDomain? Category { get; set; }

        public int? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public SupplierDomain? Supplier { get; set; }

        public int? WarehouseZoneId { get; set; }

        [ForeignKey(nameof(WarehouseZoneId))]
        public WarehouseZoneDomain? WarehouseZone { get; set; }

        public ICollection<OrderItemDomain> OrderItems { get; set; } = new List<OrderItemDomain>();

        public ICollection<ProductReviewDomain> Reviews { get; set; } = new List<ProductReviewDomain>();

        public ICollection<ProductStockReservationDomain> StockReservations { get; set; } = new List<ProductStockReservationDomain>();

        [NotMapped]
        public int AvailableQuantity => Math.Max(StockQuantity - ReservedQuantity, 0);

        [NotMapped]
        public double RatingAverage { get; set; }

        [NotMapped]
        public int RatingCount { get; set; }
    }
}
