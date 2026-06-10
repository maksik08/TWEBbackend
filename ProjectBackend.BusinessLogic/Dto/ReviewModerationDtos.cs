using System.ComponentModel.DataAnnotations;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ProductReviewListRequestDto : ListQueryRequestDto
    {
        public int? Rating { get; set; }
        public ProductReviewStatus? Status { get; set; }
        public string? Search { get; set; }
    }

    public class UploadProductReviewPhotoDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }

    public class ReportProductReviewDto
    {
        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;
    }

    public class UpdateProductReviewStatusDto
    {
        [Required]
        public ProductReviewStatus Status { get; set; }
    }

    public class ReplyToProductReviewDto
    {
        [Required]
        [MaxLength(2000)]
        public string Reply { get; set; } = string.Empty;
    }

    public class ProductReviewReportDto : AuditableDto
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int ReporterUserId { get; set; }
        public string? ReporterUsername { get; set; }
        public string Reason { get; set; } = string.Empty;
        public ProductReviewReportStatus Status { get; set; }
        public ProductReviewDto? Review { get; set; }
    }

    public class ReviewResponseTemplateDto : AuditableDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateReviewResponseTemplateDto
    {
        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Body { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class UpdateReviewResponseTemplateDto : CreateReviewResponseTemplateDto
    {
    }
}
