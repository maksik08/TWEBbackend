using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
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
    }
}
