using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
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
