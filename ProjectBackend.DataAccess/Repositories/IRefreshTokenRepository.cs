using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenDomain?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);

        Task<RefreshTokenDomain> CreateAsync(RefreshTokenDomain entity, CancellationToken cancellationToken);

        Task UpdateAsync(RefreshTokenDomain entity, CancellationToken cancellationToken);

        Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken);
    }
}
