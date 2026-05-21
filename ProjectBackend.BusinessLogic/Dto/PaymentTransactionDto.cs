using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class PaymentTransactionDto : AuditableDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public int? OrderId { get; set; }
        public PaymentTransactionType Type { get; set; }
        public PaymentTransactionStatus Status { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "MDL";
        public string? ExternalReference { get; set; }
        public string? Description { get; set; }
    }
}
