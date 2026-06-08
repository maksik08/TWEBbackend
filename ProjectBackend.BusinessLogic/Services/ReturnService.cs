using AutoMapper;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class ReturnService : ApplicationServiceBase, IReturnService
    {
        private static readonly OrderStatus[] ReturnableStatuses =
        {
            OrderStatus.Paid,
            OrderStatus.Shipped,
            OrderStatus.Completed
        };

        private readonly IReturnRepository _returnRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IActionLogService _actionLogService;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IMapper _mapper;

        public ReturnService(
            IReturnRepository returnRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IPaymentTransactionService paymentTransactionService,
            IActionLogService actionLogService,
            ICurrentUserContext currentUserContext,
            IMapper mapper)
        {
            _returnRepository = returnRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _paymentTransactionService = paymentTransactionService;
            _actionLogService = actionLogService;
            _currentUserContext = currentUserContext;
            _mapper = mapper;
        }

        public async Task<PagedResult<ReturnDto>> GetAllAsync(ReturnListRequestDto request, CancellationToken cancellationToken)
        {
            var queryOptions = new ReturnQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection ?? "desc"),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                Status = request.Status,
                OrderId = request.OrderId
            };

            var returns = await _returnRepository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<ReturnDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<ReturnDto>>(returns.Items),
                TotalCount = returns.TotalCount,
                Page = returns.Page,
                PageSize = returns.PageSize
            };
        }

        public async Task<ReturnDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = EnsureFound(await _returnRepository.GetByIdAsync(id, cancellationToken), "Return", id);
            return _mapper.Map<ReturnDto>(entity);
        }

        public async Task<ReturnDto> CreateAsync(CreateReturnDto dto, CancellationToken cancellationToken)
        {
            var order = EnsureFound(await _orderRepository.GetByIdAsync(dto.OrderId, cancellationToken), "Order", dto.OrderId);

            if (!ReturnableStatuses.Contains(order.Status))
            {
                throw new ValidationException(
                    $"A return can only be initiated for paid, shipped or completed orders. Current status: {order.Status}.");
            }

            if (await _returnRepository.HasActiveForOrderAsync(order.Id, cancellationToken))
            {
                throw new ConflictException("An active return already exists for this order.");
            }

            var entity = new ReturnDomain
            {
                OrderId = order.Id,
                UserId = order.UserId,
                Reason = NormalizeRequiredText(dto.Reason, "Reason"),
                Status = ReturnStatus.Requested,
                Amount = order.Subtotal + order.ServicesTotal
            };

            var created = await _returnRepository.CreateAsync(entity, cancellationToken);

            await _actionLogService.RecordAsync(
                "ReturnRequested",
                nameof(ReturnDomain),
                created.Id,
                $"Return requested for order {order.Id} (amount {created.Amount:0.00}).",
                cancellationToken);

            return _mapper.Map<ReturnDto>(created);
        }

        public async Task<ReturnDto> ApproveAsync(int id, ResolveReturnDto dto, CancellationToken cancellationToken)
        {
            var entity = EnsureFound(await _returnRepository.GetTrackedByIdAsync(id, cancellationToken), "Return", id);

            if (entity.Status != ReturnStatus.Requested)
            {
                throw new ValidationException($"Only requested returns can be approved. Current status: {entity.Status}.");
            }

            var order = EnsureFound(await _orderRepository.GetTrackedByIdAsync(entity.OrderId, cancellationToken), "Order", entity.OrderId);

            await using var transaction = await _orderRepository.BeginTransactionAsync(cancellationToken);

            var productIds = order.Items.Select(item => item.ProductId).Distinct().ToArray();
            var trackedProducts = await _productRepository.GetTrackedByIdsAsync(productIds, cancellationToken);
            var trackedProductsById = trackedProducts.ToDictionary(product => product.Id);

            foreach (var item in order.Items)
            {
                if (trackedProductsById.TryGetValue(item.ProductId, out var product) && !product.IsPreorder)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            await _userRepository.AdjustBalanceAsync(order.UserId, entity.Amount, cancellationToken);

            order.Status = OrderStatus.Returned;
            await _orderRepository.UpdateAsync(order, cancellationToken);

            entity.Status = ReturnStatus.Approved;
            entity.Resolution = NormalizeOptionalText(dto.Resolution);
            entity.ProcessedByUserId = _currentUserContext.UserId;
            entity.ResolvedAt = DateTime.UtcNow;
            await _returnRepository.UpdateAsync(entity, cancellationToken);

            await _paymentTransactionService.RecordAsync(
                order.UserId,
                entity.Amount,
                PaymentTransactionType.Refund,
                PaymentMethod.InternalBalance,
                PaymentTransactionStatus.Completed,
                order.Id,
                $"Refund for approved return {entity.Id} (order {order.Id})",
                null,
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            await _actionLogService.RecordAsync(
                "ReturnApproved",
                nameof(ReturnDomain),
                entity.Id,
                $"Return {entity.Id} approved; refunded {entity.Amount:0.00} for order {order.Id}.",
                cancellationToken);

            return _mapper.Map<ReturnDto>(entity);
        }

        public async Task<ReturnDto> RejectAsync(int id, ResolveReturnDto dto, CancellationToken cancellationToken)
        {
            var entity = EnsureFound(await _returnRepository.GetTrackedByIdAsync(id, cancellationToken), "Return", id);

            if (entity.Status != ReturnStatus.Requested)
            {
                throw new ValidationException($"Only requested returns can be rejected. Current status: {entity.Status}.");
            }

            entity.Status = ReturnStatus.Rejected;
            entity.Resolution = NormalizeOptionalText(dto.Resolution);
            entity.ProcessedByUserId = _currentUserContext.UserId;
            entity.ResolvedAt = DateTime.UtcNow;
            await _returnRepository.UpdateAsync(entity, cancellationToken);

            await _actionLogService.RecordAsync(
                "ReturnRejected",
                nameof(ReturnDomain),
                entity.Id,
                $"Return {entity.Id} rejected for order {entity.OrderId}.",
                cancellationToken);

            return _mapper.Map<ReturnDto>(entity);
        }
    }
}
