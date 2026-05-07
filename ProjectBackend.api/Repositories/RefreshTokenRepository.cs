using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
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
    }
}
