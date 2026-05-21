using AutoMapper;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Query;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class NotificationService : ApplicationServiceBase, INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository repository,
            ICurrentUserContext currentUserContext,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserContext = currentUserContext;
            _mapper = mapper;
        }

        public async Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(
            NotificationListRequestDto request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var queryOptions = new NotificationQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = "createdat",
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                UnreadOnly = request.UnreadOnly
            };

            var notifications = await _repository.GetForUserAsync(userId, queryOptions, cancellationToken);
            return new PagedResult<NotificationDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<NotificationDto>>(notifications.Items),
                TotalCount = notifications.TotalCount,
                Page = notifications.Page,
                PageSize = notifications.PageSize
            };
        }

        public async Task<NotificationDto> MarkAsReadAsync(int id, CancellationToken cancellationToken)
        {
            var userId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var notification = await _repository.GetTrackedByIdAsync(id, cancellationToken);
            if (notification is null || notification.UserId != userId)
            {
                throw new NotFoundException($"Notification with id {id} was not found.");
            }

            notification.IsRead = true;
            notification = await _repository.UpdateAsync(notification, cancellationToken);
            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task NotifyAsync(
            int userId,
            string title,
            string message,
            string? relatedEntityType,
            int? relatedEntityId,
            CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new NotificationDomain
            {
                UserId = userId,
                Title = title,
                Message = message,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId
            }, cancellationToken);
        }
    }
}
