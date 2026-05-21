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

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

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

        public ICollection<OrderItemDomain> OrderItems { get; set; } = new List<OrderItemDomain>();
    }
}
