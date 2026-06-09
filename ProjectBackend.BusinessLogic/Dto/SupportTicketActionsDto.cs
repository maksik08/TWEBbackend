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
    }
}
