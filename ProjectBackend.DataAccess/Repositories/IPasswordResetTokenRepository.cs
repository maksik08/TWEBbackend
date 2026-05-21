using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetTokenDomain?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);

        Task<PasswordResetTokenDomain> CreateAsync(PasswordResetTokenDomain entity, CancellationToken cancellationToken);

        Task UpdateAsync(PasswordResetTokenDomain entity, CancellationToken cancellationToken);

        Task InvalidateActiveForUserAsync(int userId, CancellationToken cancellationToken);
    }
}
