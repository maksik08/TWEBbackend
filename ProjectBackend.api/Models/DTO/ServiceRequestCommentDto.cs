namespace ProjectBackend.api.Models.DTO
{
    public class ServiceRequestCommentDto : AuditableDto
    {
        public int Id { get; set; }

        public int AuthorUserId { get; set; }

        public string AuthorUsername { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
