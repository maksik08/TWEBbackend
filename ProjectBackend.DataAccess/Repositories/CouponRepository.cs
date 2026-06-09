using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ProjectDbContext _dbContext;

        public CouponRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<CouponDomain>> GetAllAsync(CouponQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.Coupons.AsNoTracking().AsQueryable();

            if (queryOptions.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == queryOptions.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                var search = queryOptions.Search;
                query = query.Where(c => c.Code.Contains(search));
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
                : query.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<CouponDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Coupons.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<CouponDomain?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            return await _dbContext.Coupons.AsNoTracking().FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
        }

        public Task<bool> ExistsByCodeAsync(string code, int? excludedId, CancellationToken cancellationToken)
        {
            return _dbContext.Coupons.AnyAsync(
                c => c.Code == code && (excludedId == null || c.Id != excludedId),
                cancellationToken);
        }

        public async Task<CouponDomain> CreateAsync(CouponDomain entity, CancellationToken cancellationToken)
        {
            await _dbContext.Coupons.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<CouponDomain?> UpdateAsync(CouponDomain entity, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Id == entity.Id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            existing.Code = entity.Code;
            existing.DiscountType = entity.DiscountType;
            existing.DiscountValue = entity.DiscountValue;
            existing.MinOrderAmount = entity.MinOrderAmount;
            existing.MaxUses = entity.MaxUses;
            existing.ExpiresAt = entity.ExpiresAt;
            existing.IsActive = entity.IsActive;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<CouponDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            _dbContext.Coupons.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task IncrementUsageAsync(int id, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (existing is null)
            {
                return;
            }

            existing.UsedCount += 1;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
