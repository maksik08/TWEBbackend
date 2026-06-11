using AutoMapper;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.DataAccess.Repositories;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;
namespace ProjectBackend.BusinessLogic.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly ILoyaltyRepository _repo;
        private readonly IMapper _mapper;
        public LoyaltyService(ILoyaltyRepository repo, IMapper mapper) { _repo = repo; _mapper = mapper; }

        public Task<int> GetBalanceAsync(int userId, CancellationToken cancellationToken) =>
            _repo.GetByUserIdAsync(userId, cancellationToken).ContinueWith(t => t.Result?.PointsBalance ?? 0, cancellationToken);

        public async Task EarnPointsAsync(int userId, int points, string? note, CancellationToken cancellationToken)
        {
            if (points <= 0) return;
            var card = await _repo.GetByUserIdAsync(userId, cancellationToken)
                ?? await _repo.CreateAsync(new LoyaltyCardDomain { UserId = userId, PointsBalance = 0 }, cancellationToken);
            await _repo.AddTransactionAsync(new LoyaltyTransactionDomain { LoyaltyCardId = card.Id, UserId = userId, Points = points, Type = LoyaltyTransactionType.Earn, Note = note }, cancellationToken);
            await _repo.AdjustCardBalanceAsync(card.Id, points, cancellationToken);
        }

        public async Task RedeemPointsAsync(int userId, int points, string? note, CancellationToken cancellationToken)
        {
            if (points <= 0) throw new ArgumentException("Points must be positive.", nameof(points));
            var card = await _repo.GetByUserIdAsync(userId, cancellationToken) ?? throw new InvalidOperationException("Loyalty card not found.");
            if (card.PointsBalance < points) throw new InvalidOperationException("Insufficient points.");
            await _repo.AddTransactionAsync(new LoyaltyTransactionDomain { LoyaltyCardId = card.Id, UserId = userId, Points = -points, Type = LoyaltyTransactionType.Redeem, Note = note }, cancellationToken);
            await _repo.AdjustCardBalanceAsync(card.Id, -points, cancellationToken);
        }

        public async Task<PagedResult<LoyaltyTransactionDto>> GetHistoryAsync(int userId, PagedQueryOptions options, CancellationToken cancellationToken)
        {
            var history = await _repo.GetTransactionsAsync(userId, options, cancellationToken);
            return new PagedResult<LoyaltyTransactionDto> { Items = _mapper.Map<IReadOnlyCollection<LoyaltyTransactionDto>>(history.Items), TotalCount = history.TotalCount, Page = history.Page, PageSize = history.PageSize };
        }
    }
}
