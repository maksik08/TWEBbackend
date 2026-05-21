using AutoMapper;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Query;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class InstallerService : ApplicationServiceBase, IInstallerService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IWorkPhotoStorageService _workPhotoStorageService;
        private readonly INotificationService _notificationService;
        private readonly IActionLogService _actionLogService;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IMapper _mapper;

        public InstallerService(
            IServiceRequestRepository serviceRequestRepository,
            IWorkPhotoStorageService workPhotoStorageService,
            INotificationService notificationService,
            IActionLogService actionLogService,
            ICurrentUserContext currentUserContext,
            IMapper mapper)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _workPhotoStorageService = workPhotoStorageService;
            _notificationService = notificationService;
            _actionLogService = actionLogService;
            _currentUserContext = currentUserContext;
            _mapper = mapper;
        }

        public async Task<PagedResult<ServiceRequestDto>> GetMyRequestsAsync(
            ServiceRequestListRequestDto request,
            CancellationToken cancellationToken)
        {
            var installerId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var queryOptions = new ServiceRequestQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "scheduledvisitat", "scheduledvisitat", "address", "createdat"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                Status = request.Status ?? ServiceRequestStatus.Assigned,
                InstallerId = installerId,
                ScheduledFrom = request.ScheduledFrom,
                ScheduledTo = request.ScheduledTo
            };

            var requests = await _serviceRequestRepository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<ServiceRequestDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<ServiceRequestDto>>(requests.Items),
                TotalCount = requests.TotalCount,
                Page = requests.Page,
                PageSize = requests.PageSize
            };
        }

        public async Task<IReadOnlyCollection<ServiceRequestDto>> GetRouteAsync(DateTime? date, CancellationToken cancellationToken)
        {
            var installerId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var routeDate = date?.Date ?? DateTime.UtcNow.Date;
            var queryOptions = new ServiceRequestQueryOptions
            {
                Page = 1,
                PageSize = 100,
                SortBy = "scheduledvisitat",
                SortDescending = false,
                InstallerId = installerId,
                ScheduledFrom = routeDate,
                ScheduledTo = routeDate.AddDays(1).AddTicks(-1)
            };

            var requests = await _serviceRequestRepository.GetAllAsync(queryOptions, cancellationToken);
            var routeItems = requests.Items
                .Where(request => request.Status is ServiceRequestStatus.Assigned or ServiceRequestStatus.InProgress)
                .OrderBy(request => request.ScheduledVisitAt)
                .ThenBy(request => request.Address)
                .ToList();

            return _mapper.Map<IReadOnlyCollection<ServiceRequestDto>>(routeItems);
        }

        public async Task<ServiceRequestDto> StartRequestAsync(int id, CancellationToken cancellationToken)
        {
            var installerId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var request = await GetAssignedTrackedRequestAsync(id, installerId, cancellationToken);
            if (request.Status != ServiceRequestStatus.Assigned)
            {
                throw new ValidationException("Only assigned requests can be started.");
            }

            request.Status = ServiceRequestStatus.InProgress;
            request = await _serviceRequestRepository.UpdateAsync(request, cancellationToken);

            if (request.ManagerId.HasValue)
            {
                await _notificationService.NotifyAsync(
                    request.ManagerId.Value,
                    "Installer started request",
                    $"Installer started work on request {request.RequestNumber}.",
                    nameof(ServiceRequestDomain),
                    request.Id,
                    cancellationToken);
            }

            await _actionLogService.RecordAsync(
                "StartWork",
                nameof(ServiceRequestDomain),
                request.Id,
                $"Started work on request {request.RequestNumber}.",
                cancellationToken);

            return _mapper.Map<ServiceRequestDto>(request);
        }

        public async Task<ServiceRequestDto> CompleteRequestAsync(
            int id,
            CompleteServiceRequestDto dto,
            CancellationToken cancellationToken)
        {
            var installerId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var request = await GetAssignedTrackedRequestAsync(id, installerId, cancellationToken);
            if (request.Status is ServiceRequestStatus.Completed or ServiceRequestStatus.Cancelled)
            {
                throw new ValidationException("This request can no longer be completed.");
            }

            if (dto.Photos.Count is < 1 or > 3)
            {
                throw new ValidationException("You must upload from 1 to 3 work photos.");
            }

            var storedPhotos = await _workPhotoStorageService.SaveAsync(dto.Photos, cancellationToken);
            request.Status = ServiceRequestStatus.Completed;
            request.CompletionReport = NormalizeRequiredText(dto.Report, "Completion report");
            request.CompletedAt = DateTime.UtcNow;

            foreach (var storedPhoto in storedPhotos)
            {
                request.WorkPhotos.Add(new WorkPhotoDomain
                {
                    FileName = storedPhoto.FileName,
                    FilePath = storedPhoto.RelativePath
                });
            }

            request = await _serviceRequestRepository.UpdateAsync(request, cancellationToken);

            if (request.ManagerId.HasValue)
            {
                await _notificationService.NotifyAsync(
                    request.ManagerId.Value,
                    "Request completed",
                    $"Installer completed request {request.RequestNumber}.",
                    nameof(ServiceRequestDomain),
                    request.Id,
                    cancellationToken);
            }

            await _notificationService.NotifyAsync(
                request.CustomerId,
                "Connection request completed",
                $"Your request {request.RequestNumber} has been completed.",
                nameof(ServiceRequestDomain),
                request.Id,
                cancellationToken);

            await _actionLogService.RecordAsync(
                "CompleteWork",
                nameof(ServiceRequestDomain),
                request.Id,
                $"Completed request {request.RequestNumber} with {storedPhotos.Count} photo(s).",
                cancellationToken);

            return _mapper.Map<ServiceRequestDto>(request);
        }

        private async Task<ServiceRequestDomain> GetAssignedTrackedRequestAsync(
            int id,
            int installerId,
            CancellationToken cancellationToken)
        {
            var request = EnsureFound(await _serviceRequestRepository.GetTrackedByIdAsync(id, cancellationToken), "Service request", id);
            if (request.InstallerId != installerId)
            {
                throw new NotFoundException($"Service request with id {id} was not found.");
            }

            return request;
        }
    }
}
