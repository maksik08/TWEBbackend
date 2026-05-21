using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IPaymentTransactionRepository
    {
        Task<PagedResult<PaymentTransactionDomain>> GetForUserAsync(int userId, PaymentTransactionQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<PagedResult<PaymentTransactionDomain>> GetAllAsync(PaymentTransactionQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<PaymentTransactionDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<PaymentTransactionDomain> CreateAsync(PaymentTransactionDomain paymentTransaction, CancellationToken cancellationToken);
    }
}
