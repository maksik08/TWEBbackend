using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class SupportMessageDto : AuditableDto
    {
        public int Id { get; set; }
        public int AuthorUserId { get; set; }
        public string? AuthorUsername { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class SupportTicketDto : AuditableDto
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public SupportTicketStatus Status { get; set; }
        public SupportPriority Priority { get; set; }
        public DateTime? EscalatedAt { get; set; }
        public byte? SatisfactionRating { get; set; }
        public string? SatisfactionComment { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerUsername { get; set; }
        public int? AssignedAgentId { get; set; }
        public string? AssignedAgentUsername { get; set; }
        public IReadOnlyCollection<SupportMessageDto> Messages { get; set; } = new List<SupportMessageDto>();
        public IReadOnlyCollection<SupportAttachmentDto> Attachments { get; set; } = new List<SupportAttachmentDto>();
    }

    public class SupportAttachmentDto : AuditableDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UploadedByUserId { get; set; }
        public string? UploadedByUsername { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}
