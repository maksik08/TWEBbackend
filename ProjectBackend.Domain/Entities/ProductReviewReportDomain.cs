using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class ProductReviewReportDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ReviewId { get; set; }

        [ForeignKey(nameof(ReviewId))]
        public ProductReviewDomain? Review { get; set; }

        public int ReporterUserId { get; set; }

        [ForeignKey(nameof(ReporterUserId))]
        public UserDomain? ReporterUser { get; set; }

        [MaxLength(1000)]
        public required string Reason { get; set; }

        public ProductReviewReportStatus Status { get; set; } = ProductReviewReportStatus.Open;
    }
}
