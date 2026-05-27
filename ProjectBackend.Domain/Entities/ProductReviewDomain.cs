using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class ProductReviewDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductsDomain? Product { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserDomain? User { get; set; }

        public byte Rating { get; set; }

        [MaxLength(2000)]
        public required string Comment { get; set; }
    }
}
