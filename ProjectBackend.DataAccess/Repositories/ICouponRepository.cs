using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface ICouponRepository
    {
        Task<PagedResult<CouponDomain>> GetAllAsync(CouponQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<CouponDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CouponDomain?> GetByCodeAsync(string code, CancellationToken cancellationToken);
        Task<bool> ExistsByCodeAsync(string code, int? excludedId, CancellationToken cancellationToken);
        Task<CouponDomain> CreateAsync(CouponDomain entity, CancellationToken cancellationToken);
        Task<CouponDomain?> UpdateAsync(CouponDomain entity, CancellationToken cancellationToken);
        Task<CouponDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
        Task IncrementUsageAsync(int id, CancellationToken cancellationToken);
    }
}
