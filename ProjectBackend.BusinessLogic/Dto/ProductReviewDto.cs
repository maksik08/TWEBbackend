using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ProductReviewDto : AuditableDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string AuthorUsername { get; set; } = string.Empty;
        public byte Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public ProductReviewStatus Status { get; set; }
        public string? ModeratorReply { get; set; }
        public int? ModeratorUserId { get; set; }
        public string? ModeratorUsername { get; set; }
        public DateTime? RepliedAt { get; set; }
        public IReadOnlyCollection<ProductReviewPhotoDto> Photos { get; set; } = new List<ProductReviewPhotoDto>();
    }

    public class ProductReviewPhotoDto : AuditableDto
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}
