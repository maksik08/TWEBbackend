using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface INotificationService
    {
        Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(NotificationListRequestDto request, CancellationToken cancellationToken);
        Task<NotificationDto> MarkAsReadAsync(int id, CancellationToken cancellationToken);
        Task NotifyAsync(int userId, string title, string message, string? relatedEntityType, int? relatedEntityId, CancellationToken cancellationToken);
    }
}
