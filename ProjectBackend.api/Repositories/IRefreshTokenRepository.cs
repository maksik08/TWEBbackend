using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenDomain?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);

        Task<RefreshTokenDomain> CreateAsync(RefreshTokenDomain entity, CancellationToken cancellationToken);

        Task UpdateAsync(RefreshTokenDomain entity, CancellationToken cancellationToken);
    }
}
