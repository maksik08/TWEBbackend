using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public class ActionLogRepository : IActionLogRepository
    {
        private readonly ProjectDbContext _dbContext;

        public ActionLogRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActionLogDomain> CreateAsync(ActionLogDomain actionLog, CancellationToken cancellationToken)
        {
            await _dbContext.ActionLogs.AddAsync(actionLog, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return actionLog;
        }

        public async Task<PagedResult<ActionLogDomain>> GetAllAsync(ActionLogQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.ActionLogs
                .AsNoTracking()
                .Include(log => log.ActorUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryOptions.EntityType))
            {
                query = query.Where(log => log.EntityType == queryOptions.EntityType);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Action))
            {
                query = query.Where(log => log.Action == queryOptions.Action);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                var search = queryOptions.Search;
                query = query.Where(log =>
                    (log.Details != null && log.Details.Contains(search)) ||
                    log.Action.Contains(search) ||
                    log.EntityType.Contains(search) ||
                    (log.ActorUser != null && log.ActorUser.Username.Contains(search)));
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(log => log.CreatedAt).ThenByDescending(log => log.Id)
                : query.OrderBy(log => log.CreatedAt).ThenBy(log => log.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }
    }
}
