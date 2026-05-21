using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly ProjectDbContext _dbContext;

        public PasswordResetTokenRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<PasswordResetTokenDomain?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken)
        {
            return _dbContext.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
        }

        public async Task<PasswordResetTokenDomain> CreateAsync(PasswordResetTokenDomain entity, CancellationToken cancellationToken)
        {
            await _dbContext.PasswordResetTokens.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public Task UpdateAsync(PasswordResetTokenDomain entity, CancellationToken cancellationToken)
        {
            _dbContext.PasswordResetTokens.Update(entity);
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task InvalidateActiveForUserAsync(int userId, CancellationToken cancellationToken)
        {
            var utcNow = DateTime.UtcNow;
            var active = await _dbContext.PasswordResetTokens
                .Where(t => t.UserId == userId && t.ConsumedAt == null && t.ExpiresAt > utcNow)
                .ToListAsync(cancellationToken);

            if (active.Count == 0)
            {
                return;
            }

            foreach (var token in active)
            {
                token.ConsumedAt = utcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
