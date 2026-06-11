using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Common;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface ILoyaltyService
    {
        Task<int> GetBalanceAsync(int userId, CancellationToken cancellationToken);
        Task EarnPointsAsync(int userId, int points, string? note, CancellationToken cancellationToken);
        Task RedeemPointsAsync(int userId, int points, string? note, CancellationToken cancellationToken);
        Task<PagedResult<LoyaltyTransactionDto>> GetHistoryAsync(int userId, PagedQueryOptions options, CancellationToken cancellationToken);
    }
}
