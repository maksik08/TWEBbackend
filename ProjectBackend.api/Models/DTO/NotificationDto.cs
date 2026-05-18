namespace ProjectBackend.api.Models.DTO
{
    public class NotificationDto : AuditableDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        public bool IsRead { get; set; }
    }
}
