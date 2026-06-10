using System.ComponentModel.DataAnnotations;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class KnowledgeBaseArticleDto : AuditableDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public KnowledgeBaseArticleStatus Status { get; set; }
    }

    public class KnowledgeBaseArticleListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public KnowledgeBaseArticleStatus? Status { get; set; }
    }

    public class CreateKnowledgeBaseArticleDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(4000)]
        public string Content { get; set; } = string.Empty;

        public KnowledgeBaseArticleStatus Status { get; set; } = KnowledgeBaseArticleStatus.Published;
    }

    public class UpdateKnowledgeBaseArticleDto : CreateKnowledgeBaseArticleDto
    {
    }

    public class WarrantyCheckDto
    {
        public int ProductId { get; set; }
        public int? OrderId { get; set; }
        public bool WarrantyValid { get; set; }
        public DateTime? PurchasedAt { get; set; }
        public DateTime? WarrantyExpiresAt { get; set; }
        public string? WarrantySource { get; set; }
    }

    public class CreateWarrantyClaimDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        public int? OrderId { get; set; }

        public int? SupportTicketId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string IssueDescription { get; set; } = string.Empty;
    }

    public class UpdateWarrantyClaimStatusDto
    {
        public WarrantyClaimStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Resolution { get; set; }
    }

    public class WarrantyClaimDto : AuditableDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerUsername { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? OrderId { get; set; }
        public int? SupportTicketId { get; set; }
        public WarrantyClaimStatus Status { get; set; }
        public bool WarrantyValid { get; set; }
        public DateTime? PurchasedAt { get; set; }
        public DateTime? WarrantyExpiresAt { get; set; }
        public string IssueDescription { get; set; } = string.Empty;
        public string? Resolution { get; set; }
    }

    public class StartRemoteDiagnosticDto
    {
        public DateTime? ScheduledAt { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }
    }

    public class CompleteRemoteDiagnosticDto
    {
        [Required]
        [MaxLength(2000)]
        public string Result { get; set; } = string.Empty;
    }

    public class RemoteDiagnosticSessionDto : AuditableDto
    {
        public int Id { get; set; }
        public int SupportTicketId { get; set; }
        public int AgentId { get; set; }
        public string? AgentUsername { get; set; }
        public RemoteDiagnosticStatus Status { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }
        public string? Result { get; set; }
    }
}
