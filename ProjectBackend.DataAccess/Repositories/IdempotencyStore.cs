using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public class IdempotencyStore : IIdempotencyStore
    {
        private readonly ProjectDbContext _dbContext;

        public IdempotencyStore(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<IdempotencyRecordDomain?> GetAsync(int? userId, string key, string method, string path, CancellationToken cancellationToken)
        {
            var utcNow = DateTime.UtcNow;

            return _dbContext.IdempotencyRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    r => r.UserId == userId
                        && r.Key == key
                        && r.Method == method
                        && r.Path == path
                        && r.ExpiresAt > utcNow,
                    cancellationToken);
        }

        public async Task<bool> TrySaveAsync(IdempotencyRecordDomain record, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.IdempotencyRecords.AddAsync(record, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateException)
            {
                _dbContext.Entry(record).State = EntityState.Detached;
                return false;
            }
        }
    }
}
