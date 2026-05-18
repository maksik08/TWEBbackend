using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Authenticated user notification endpoints.
    /// Access: authenticated user, staff, or admin.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [UserAccess]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy([FromQuery] NotificationListRequestDto request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationService.GetMyNotificationsAsync(request, cancellationToken);
            return Ok(PagedResponse<NotificationDto>.Ok(notifications));
        }

        [HttpPatch("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] int id, CancellationToken cancellationToken)
        {
            var notification = await _notificationService.MarkAsReadAsync(id, cancellationToken);
            return Ok(ApiResponse<NotificationDto>.Ok(notification, "Notification marked as read."));
        }
    }
}
