using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.DTO
{
    public class PaymentTransactionListRequestDto : ListQueryRequestDto
    {
        public PaymentTransactionType? Type { get; set; }
        public PaymentTransactionStatus? Status { get; set; }
        public int? OrderId { get; set; }
    }
}
