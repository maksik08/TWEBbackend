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
    }
}
