using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ReturnDto : AuditableDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string Reason { get; set; } = string.Empty;
        public ReturnStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string? Resolution { get; set; }
        public int? ProcessedByUserId { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
