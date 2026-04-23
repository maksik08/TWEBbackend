using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.api.Models.Domain

{
    public class ProductsDomain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; } = 0;

        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public CategoryDomain? Category { get; set; }

        public int? SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        public SupplierDomain? Supplier { get; set; }
    }
}
