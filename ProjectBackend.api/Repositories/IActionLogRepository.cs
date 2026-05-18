using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public interface IActionLogRepository
    {
        Task<ActionLogDomain> CreateAsync(ActionLogDomain actionLog, CancellationToken cancellationToken);
    }
}
