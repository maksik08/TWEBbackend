using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class ProductReviewPhotoDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ReviewId { get; set; }

        [ForeignKey(nameof(ReviewId))]
        public ProductReviewDomain? Review { get; set; }

        [MaxLength(260)]
        public required string FileName { get; set; }

        [MaxLength(500)]
        public required string FilePath { get; set; }
    }
}
