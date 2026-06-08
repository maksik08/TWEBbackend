using AutoMapper;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class ActionLogService : IActionLogService
    {
        private readonly IActionLogRepository _repository;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IMapper _mapper;

        public ActionLogService(
            IActionLogRepository repository,
            ICurrentUserContext currentUserContext,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserContext = currentUserContext;
            _mapper = mapper;
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

        public async Task<PagedResult<ActionLogDto>> GetAllAsync(ActionLogListRequestDto request, CancellationToken cancellationToken)
        {
            var queryOptions = new ActionLogQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection ?? "desc"),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                EntityType = QueryValidationHelper.NormalizeSearch(request.EntityType),
                Action = QueryValidationHelper.NormalizeSearch(request.Action)
            };

            var logs = await _repository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<ActionLogDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<ActionLogDto>>(logs.Items),
                TotalCount = logs.TotalCount,
                Page = logs.Page,
                PageSize = logs.PageSize
            };
        }
    }
}
