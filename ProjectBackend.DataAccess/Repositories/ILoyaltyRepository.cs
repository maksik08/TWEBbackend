using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface ILoyaltyRepository
    {
        Task<LoyaltyCardDomain?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<LoyaltyCardDomain> CreateAsync(LoyaltyCardDomain card, CancellationToken cancellationToken);
        Task AddTransactionAsync(LoyaltyTransactionDomain transaction, CancellationToken cancellationToken);
        Task<PagedResult<LoyaltyTransactionDomain>> GetTransactionsAsync(int userId, PagedQueryOptions options, CancellationToken cancellationToken);
        Task AdjustCardBalanceAsync(int cardId, int delta, CancellationToken cancellationToken);
    }
}
