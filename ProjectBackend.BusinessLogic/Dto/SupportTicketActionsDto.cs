using System.ComponentModel.DataAnnotations;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class UpdateSupportTicketStatusDto
    {
        [Required]
        [EnumDataType(typeof(SupportTicketStatus))]
        public SupportTicketStatus Status { get; set; }
    }

    public class AssignSupportAgentDto
    {
        [Range(1, int.MaxValue)]
        public int AgentId { get; set; }
    }

    public class SupportTicketListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }

        public SupportTicketStatus? Status { get; set; }

        public SupportPriority? Priority { get; set; }
    }

    public class EscalateSupportTicketDto
    {
        [Required]
        [EnumDataType(typeof(SupportPriority))]
        public SupportPriority Priority { get; set; } = SupportPriority.High;

        [MaxLength(1000)]
        public string? Reason { get; set; }
    }

    public class RateSupportTicketDto
    {
        [Range(1, 5)]
        public byte Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }

    public class UploadSupportAttachmentDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
