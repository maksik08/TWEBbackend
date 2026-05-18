using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IPaymentTransactionService
    {
        Task<PagedResult<PaymentTransactionDto>> GetMyPaymentsAsync(PaymentTransactionListRequestDto request, CancellationToken cancellationToken);
        Task<PagedResult<PaymentTransactionDto>> GetAllAsync(PaymentTransactionListRequestDto request, CancellationToken cancellationToken);
        Task<PaymentTransactionDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<PaymentTransactionDto> RecordAsync(
            int userId,
            decimal amount,
            PaymentTransactionType type,
            PaymentMethod method,
            PaymentTransactionStatus status,
            int? orderId,
            string? description,
            string? externalReference,
            CancellationToken cancellationToken);
    }
}
