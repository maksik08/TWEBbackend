using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface INotificationRepository
    {
        Task<PagedResult<NotificationDomain>> GetForUserAsync(int userId, NotificationQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<NotificationDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken);
        Task<NotificationDomain> CreateAsync(NotificationDomain notification, CancellationToken cancellationToken);
        Task<NotificationDomain> UpdateAsync(NotificationDomain notification, CancellationToken cancellationToken);
    }
}
