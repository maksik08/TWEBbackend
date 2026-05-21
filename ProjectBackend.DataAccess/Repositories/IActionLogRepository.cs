using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IActionLogRepository
    {
        Task<ActionLogDomain> CreateAsync(ActionLogDomain actionLog, CancellationToken cancellationToken);
    }
}
