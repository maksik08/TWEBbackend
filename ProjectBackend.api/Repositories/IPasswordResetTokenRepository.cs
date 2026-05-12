using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetTokenDomain?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);

        Task<PasswordResetTokenDomain> CreateAsync(PasswordResetTokenDomain entity, CancellationToken cancellationToken);

        Task UpdateAsync(PasswordResetTokenDomain entity, CancellationToken cancellationToken);

        Task InvalidateActiveForUserAsync(int userId, CancellationToken cancellationToken);
    }
}
