using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface INotificationService
    {
        Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(NotificationListRequestDto request, CancellationToken cancellationToken);
        Task<NotificationDto> MarkAsReadAsync(int id, CancellationToken cancellationToken);
        Task NotifyAsync(int userId, string title, string message, string? relatedEntityType, int? relatedEntityId, CancellationToken cancellationToken);
    }
}
