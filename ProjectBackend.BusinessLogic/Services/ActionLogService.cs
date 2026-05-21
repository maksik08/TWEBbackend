using ProjectBackend.Domain.Entities;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class ActionLogService : IActionLogService
    {
        private readonly IActionLogRepository _repository;
        private readonly ICurrentUserContext _currentUserContext;

        public ActionLogService(IActionLogRepository repository, ICurrentUserContext currentUserContext)
        {
            _repository = repository;
            _currentUserContext = currentUserContext;
        }

        public async Task RecordAsync(string action, string entityType, int? entityId, string? details, CancellationToken cancellationToken)
        {
            var role = _currentUserContext.Role?.ToString() ?? UserRole.Guest.ToString();
            await _repository.CreateAsync(new ActionLogDomain
            {
                ActorUserId = _currentUserContext.UserId,
                ActorRole = role,
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                Details = details
            }, cancellationToken);
        }
    }
}
