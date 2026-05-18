using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.Query
{
    public sealed class PaymentTransactionQueryOptions : PagedQueryOptions
    {
        public PaymentTransactionType? Type { get; init; }
        public PaymentTransactionStatus? Status { get; init; }
        public int? OrderId { get; init; }
    }
}
