using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class GoodsReceiptItemDomain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int GoodsReceiptId { get; set; }

        [ForeignKey(nameof(GoodsReceiptId))]
        public GoodsReceiptDomain? GoodsReceipt { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductsDomain? Product { get; set; }

        public int AcceptedQuantity { get; set; }

        public int RejectedQuantity { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
