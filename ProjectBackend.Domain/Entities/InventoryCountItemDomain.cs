using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class InventoryCountItemDomain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int InventoryCountId { get; set; }

        [ForeignKey(nameof(InventoryCountId))]
        public InventoryCountDomain? InventoryCount { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductsDomain? Product { get; set; }

        public int SystemQuantity { get; set; }

        public int CountedQuantity { get; set; }

        public int Variance { get; set; }
    }
}
