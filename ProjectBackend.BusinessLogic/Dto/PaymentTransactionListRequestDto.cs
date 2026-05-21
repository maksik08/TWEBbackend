using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class PaymentTransactionListRequestDto : ListQueryRequestDto
    {
        public PaymentTransactionType? Type { get; set; }
        public PaymentTransactionStatus? Status { get; set; }
        public int? OrderId { get; set; }
    }
}
