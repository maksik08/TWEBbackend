using AutoMapper;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Query;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class ServiceRequestService : ApplicationServiceBase, IServiceRequestService
    {
        private readonly IServiceRequestRepository _repository;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IActionLogService _actionLogService;
        private readonly IMapper _mapper;

        public ServiceRequestService(
            IServiceRequestRepository repository,
            ICurrentUserContext currentUserContext,
            IActionLogService actionLogService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserContext = currentUserContext;
            _actionLogService = actionLogService;
            _mapper = mapper;
        }

        public async Task<PagedResult<ServiceRequestDto>> GetMyRequestsAsync(
            ServiceRequestListRequestDto request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var queryOptions = new ServiceRequestQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat", "scheduledvisitat", "address"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                Status = request.Status,
                CustomerId = userId,
                ScheduledFrom = request.ScheduledFrom,
                ScheduledTo = request.ScheduledTo
            };

            var requests = await _repository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<ServiceRequestDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<ServiceRequestDto>>(requests.Items),
                TotalCount = requests.TotalCount,
                Page = requests.Page,
                PageSize = requests.PageSize
            };
        }

        public async Task<ServiceRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var userId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var request = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Service request", id);
            if (request.CustomerId != userId)
            {
                throw new NotFoundException($"Service request with id {id} was not found.");
            }

            return _mapper.Map<ServiceRequestDto>(request);
        }

        public async Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto, CancellationToken cancellationToken)
        {
            var userId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var serviceRequest = new ServiceRequestDomain
            {
                RequestNumber = GenerateRequestNumber(),
                CustomerId = userId,
                ServiceTitle = NormalizeRequiredText(dto.ServiceTitle, "Service title"),
                Description = NormalizeOptionalText(dto.Description),
                Address = NormalizeRequiredText(dto.Address, "Address"),
                ContactPhone = NormalizeRequiredText(dto.ContactPhone, "Contact phone"),
                PreferredVisitAt = dto.PreferredVisitAt,
                Status = ServiceRequestStatus.Submitted
            };

            var created = await _repository.CreateAsync(serviceRequest, cancellationToken);
            await _actionLogService.RecordAsync(
                "Create",
                nameof(ServiceRequestDomain),
                created.Id,
                $"Created service request {created.RequestNumber}.",
                cancellationToken);

            return _mapper.Map<ServiceRequestDto>(created);
        }

        private static string GenerateRequestNumber()
        {
            return $"REQ-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        }
    }
}
