using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IActionLogRepository
    {
        Task<ActionLogDomain> CreateAsync(ActionLogDomain actionLog, CancellationToken cancellationToken);
        Task<PagedResult<ActionLogDomain>> GetAllAsync(ActionLogQueryOptions queryOptions, CancellationToken cancellationToken);
    }
}
