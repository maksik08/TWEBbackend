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

        public ProductReviewStatus Status { get; set; } = ProductReviewStatus.Published;

        [MaxLength(2000)]
        public string? ModeratorReply { get; set; }

        public int? ModeratorUserId { get; set; }

        [ForeignKey(nameof(ModeratorUserId))]
        public UserDomain? ModeratorUser { get; set; }

        public DateTime? RepliedAt { get; set; }

        public ICollection<ProductReviewPhotoDomain> Photos { get; set; } = new List<ProductReviewPhotoDomain>();

        public ICollection<ProductReviewReportDomain> Reports { get; set; } = new List<ProductReviewReportDomain>();
    }
}
