using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class SupplierReturnItemDomain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SupplierReturnId { get; set; }

        [ForeignKey(nameof(SupplierReturnId))]
        public SupplierReturnDomain? SupplierReturn { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductsDomain? Product { get; set; }

        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
