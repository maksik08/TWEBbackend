using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ProjectDbContext _dbContext;

        public NotificationRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<NotificationDomain>> GetForUserAsync(
            int userId,
            NotificationQueryOptions queryOptions,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.Notifications
                .AsNoTracking()
                .Where(notification => notification.UserId == userId);

            if (queryOptions.UnreadOnly)
            {
                query = query.Where(notification => !notification.IsRead);
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(notification => notification.CreatedAt).ThenByDescending(notification => notification.Id)
                : query.OrderBy(notification => notification.CreatedAt).ThenBy(notification => notification.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<NotificationDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Notifications.FirstOrDefaultAsync(notification => notification.Id == id, cancellationToken);
        }

        public async Task<NotificationDomain> CreateAsync(NotificationDomain notification, CancellationToken cancellationToken)
        {
            await _dbContext.Notifications.AddAsync(notification, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return notification;
        }

        public async Task<NotificationDomain> UpdateAsync(NotificationDomain notification, CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return notification;
        }
    }
}
