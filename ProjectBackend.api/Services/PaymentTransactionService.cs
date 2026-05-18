using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Models.Query;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class PaymentTransactionService : ApplicationServiceBase, IPaymentTransactionService
    {
        private readonly IPaymentTransactionRepository _repository;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IMapper _mapper;

        public PaymentTransactionService(
            IPaymentTransactionRepository repository,
            ICurrentUserContext currentUserContext,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserContext = currentUserContext;
            _mapper = mapper;
        }

        public async Task<PagedResult<PaymentTransactionDto>> GetMyPaymentsAsync(
            PaymentTransactionListRequestDto request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var queryOptions = BuildQueryOptions(request);
            var payments = await _repository.GetForUserAsync(userId, queryOptions, cancellationToken);

            return new PagedResult<PaymentTransactionDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<PaymentTransactionDto>>(payments.Items),
                TotalCount = payments.TotalCount,
                Page = payments.Page,
                PageSize = payments.PageSize
            };
        }

        public async Task<PagedResult<PaymentTransactionDto>> GetAllAsync(
            PaymentTransactionListRequestDto request,
            CancellationToken cancellationToken)
        {
            var queryOptions = BuildQueryOptions(request);
            var payments = await _repository.GetAllAsync(queryOptions, cancellationToken);

            return new PagedResult<PaymentTransactionDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<PaymentTransactionDto>>(payments.Items),
                TotalCount = payments.TotalCount,
                Page = payments.Page,
                PageSize = payments.PageSize
            };
        }

        public async Task<PaymentTransactionDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var payment = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Payment transaction", id);
            if (_currentUserContext.Role != UserRole.Admin && payment.UserId != _currentUserContext.UserId)
            {
                throw new NotFoundException($"Payment transaction with id {id} was not found.");
            }

            return _mapper.Map<PaymentTransactionDto>(payment);
        }

        public async Task<PaymentTransactionDto> RecordAsync(
            int userId,
            decimal amount,
            PaymentTransactionType type,
            PaymentMethod method,
            PaymentTransactionStatus status,
            int? orderId,
            string? description,
            string? externalReference,
            CancellationToken cancellationToken)
        {
            EnsureMinimumValue(amount, 0.01m, "Payment amount");

            var payment = new PaymentTransactionDomain
            {
                UserId = userId,
                OrderId = orderId,
                Amount = amount,
                Type = type,
                Method = method,
                Status = status,
                Description = NormalizeOptionalText(description),
                ExternalReference = NormalizeOptionalText(externalReference)
            };

            var created = await _repository.CreateAsync(payment, cancellationToken);
            return _mapper.Map<PaymentTransactionDto>(created);
        }

        private static PaymentTransactionQueryOptions BuildQueryOptions(PaymentTransactionListRequestDto request)
        {
            return new PaymentTransactionQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat", "amount", "type", "status"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Type = request.Type,
                Status = request.Status,
                OrderId = request.OrderId
            };
        }
    }
}
