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
        private readonly IServiceTariffRepository _tariffRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IActionLogService _actionLogService;
        private readonly IMapper _mapper;

        public ServiceRequestService(
            IServiceRequestRepository repository,
            IServiceTariffRepository tariffRepository,
            IUserRepository userRepository,
            IPaymentTransactionService paymentTransactionService,
            ICurrentUserContext currentUserContext,
            IActionLogService actionLogService,
            IMapper mapper)
        {
            _repository = repository;
            _tariffRepository = tariffRepository;
            _userRepository = userRepository;
            _paymentTransactionService = paymentTransactionService;
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

            var tariff = EnsureFound(
                await _tariffRepository.GetByIdAsync(dto.ServiceTariffId, cancellationToken),
                "Service tariff",
                dto.ServiceTariffId);

            if (!tariff.IsActive)
            {
                throw new ValidationException("This service tariff is not available.");
            }

            var serviceRequest = new ServiceRequestDomain
            {
                RequestNumber = GenerateRequestNumber(),
                CustomerId = userId,
                ServiceTariffId = tariff.Id,
                ServiceTitle = tariff.Name,
                Price = tariff.Price,
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
                $"Created service request {created.RequestNumber} ({tariff.Name}, {tariff.Price:0.00}).",
                cancellationToken);

            return _mapper.Map<ServiceRequestDto>(created);
        }

        public async Task<ServiceRequestDto> PayAsync(int id, CancellationToken cancellationToken)
        {
            var userId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var request = EnsureFound(await _repository.GetTrackedByIdAsync(id, cancellationToken), "Service request", id);
            if (request.CustomerId != userId)
            {
                throw new NotFoundException($"Service request with id {id} was not found.");
            }

            if (request.PaidAt.HasValue)
            {
                throw new ValidationException("This service request has already been paid.");
            }

            if (request.Status == ServiceRequestStatus.Cancelled)
            {
                throw new ValidationException("A cancelled service request cannot be paid.");
            }

            if (request.Price <= 0)
            {
                throw new ValidationException("This service request has no payable amount.");
            }

            var payer = EnsureFound(await _userRepository.GetByIdAsync(userId, cancellationToken), "User", userId);
            if (payer.Balance < request.Price)
            {
                throw new ValidationException("Insufficient balance to pay for this service.");
            }

            await _userRepository.AdjustBalanceAsync(userId, -request.Price, cancellationToken);

            request.PaidAt = DateTime.UtcNow;
            var updated = await _repository.UpdateAsync(request, cancellationToken);

            await _paymentTransactionService.RecordAsync(
                userId,
                request.Price,
                PaymentTransactionType.ServicePayment,
                PaymentMethod.InternalBalance,
                PaymentTransactionStatus.Completed,
                null,
                $"Payment for service request {request.RequestNumber}",
                null,
                cancellationToken);

            await _actionLogService.RecordAsync(
                "Pay",
                nameof(ServiceRequestDomain),
                updated.Id,
                $"Paid service request {updated.RequestNumber} for {updated.Price:0.00}.",
                cancellationToken);

            return _mapper.Map<ServiceRequestDto>(updated);
        }

        public async Task<ServiceRequestDto> RateAsync(int id, RateServiceRequestDto dto, CancellationToken cancellationToken)
        {
            var userId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                throw new ValidationException("Rating must be between 1 and 5.");
            }

            var request = EnsureFound(await _repository.GetTrackedByIdAsync(id, cancellationToken), "Service request", id);
            if (request.CustomerId != userId)
            {
                throw new NotFoundException($"Service request with id {id} was not found.");
            }

            if (request.Status != ServiceRequestStatus.Completed)
            {
                throw new ValidationException("Only completed service requests can be rated.");
            }

            request.Rating = dto.Rating;
            request.RatingComment = NormalizeOptionalText(dto.Comment);
            request.RatedAt = DateTime.UtcNow;
            var updated = await _repository.UpdateAsync(request, cancellationToken);

            await _actionLogService.RecordAsync(
                "Rate",
                nameof(ServiceRequestDomain),
                updated.Id,
                $"Rated service request {updated.RequestNumber}: {dto.Rating}/5.",
                cancellationToken);

            return _mapper.Map<ServiceRequestDto>(updated);
        }

        private static string GenerateRequestNumber()
        {
            return $"REQ-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        }
    }
}
