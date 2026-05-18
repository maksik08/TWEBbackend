using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public interface IPaymentTransactionRepository
    {
        Task<PagedResult<PaymentTransactionDomain>> GetForUserAsync(int userId, PaymentTransactionQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<PagedResult<PaymentTransactionDomain>> GetAllAsync(PaymentTransactionQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<PaymentTransactionDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<PaymentTransactionDomain> CreateAsync(PaymentTransactionDomain paymentTransaction, CancellationToken cancellationToken);
    }
}
