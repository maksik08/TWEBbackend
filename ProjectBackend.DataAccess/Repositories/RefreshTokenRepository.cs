using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ProjectDbContext _dbContext;

        public RefreshTokenRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<RefreshTokenDomain?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken)
        {
            return _dbContext.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
        }

        public async Task<RefreshTokenDomain> CreateAsync(RefreshTokenDomain entity, CancellationToken cancellationToken)
        {
            await _dbContext.RefreshTokens.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public Task UpdateAsync(RefreshTokenDomain entity, CancellationToken cancellationToken)
        {
            _dbContext.RefreshTokens.Update(entity);
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken)
        {
            var utcNow = DateTime.UtcNow;
            var active = await _dbContext.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > utcNow)
                .ToListAsync(cancellationToken);

            if (active.Count == 0)
            {
                return;
            }

            foreach (var token in active)
            {
                token.RevokedAt = utcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
