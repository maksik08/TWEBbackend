using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public interface INotificationRepository
    {
        Task<PagedResult<NotificationDomain>> GetForUserAsync(int userId, NotificationQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<NotificationDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken);
        Task<NotificationDomain> CreateAsync(NotificationDomain notification, CancellationToken cancellationToken);
        Task<NotificationDomain> UpdateAsync(NotificationDomain notification, CancellationToken cancellationToken);
    }
}
