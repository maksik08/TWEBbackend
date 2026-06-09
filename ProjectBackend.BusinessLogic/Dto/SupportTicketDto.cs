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
        public int CustomerId { get; set; }
        public string? CustomerUsername { get; set; }
        public int? AssignedAgentId { get; set; }
        public string? AssignedAgentUsername { get; set; }
        public IReadOnlyCollection<SupportMessageDto> Messages { get; set; } = new List<SupportMessageDto>();
    }
}
