using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Models.Query;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class OrderService : ApplicationServiceBase, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IActionLogService _actionLogService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            ICurrentUserContext currentUserContext,
            IActionLogService actionLogService,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _currentUserContext = currentUserContext;
            _actionLogService = actionLogService;
            _mapper = mapper;
        }

        public Task<PagedResult<OrderDto>> GetMineAsync(OrderListRequestDto request, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();
            return ListAsync(request, userId, cancellationToken);
        }

        public Task<PagedResult<OrderDto>> GetAllAsync(OrderListRequestDto request, CancellationToken cancellationToken)
        {
            return ListAsync(request, request.UserId ?? request.CustomerId, cancellationToken);
        }

        public async Task<OrderDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var order = EnsureFound(await _orderRepository.GetByIdAsync(id, cancellationToken), "Order", id);
            EnsureCanAccess(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();

            if (dto.Items.Count == 0)
            {
                throw new ValidationException("Order must contain at least one item.");
            }

            var aggregated = dto.Items
                .GroupBy(item => item.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Quantity = group.Sum(item => item.Quantity)
                })
                .ToList();

            foreach (var line in aggregated)
            {
                if (line.Quantity <= 0)
                {
                    throw new ValidationException($"Quantity for product {line.ProductId} must be greater than zero.");
                }
            }

            var productIds = aggregated.Select(line => line.ProductId).ToArray();
            var products = await _productRepository.GetByIdsAsync(productIds, includeHidden: false, cancellationToken);
            if (products.Count != productIds.Length)
            {
                throw new ValidationException("One or more selected products do not exist or are not available.");
            }

            var productsById = products.ToDictionary(product => product.Id);
            var insufficient = aggregated
                .Where(line =>
                {
                    var product = productsById[line.ProductId];
                    return !product.IsPreorder && product.StockQuantity < line.Quantity;
                })
                .Select(line =>
                {
                    var product = productsById[line.ProductId];
                    return $"{product.Title ?? product.Name} (requested {line.Quantity}, available {product.StockQuantity})";
                })
                .ToList();

            if (insufficient.Count > 0)
            {
                throw new ValidationException($"Not enough stock for: {string.Join("; ", insufficient)}.");
            }

            var order = new OrderDomain
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                RecipientName = NormalizeRequiredText(dto.RecipientName, "Recipient name"),
                Phone = NormalizeRequiredText(dto.Phone, "Phone"),
                ShippingAddress = NormalizeRequiredText(dto.ShippingAddress, "Shipping address"),
                City = NormalizeRequiredText(dto.City, "City"),
                Comment = NormalizeOptionalText(dto.Comment),
                Items = aggregated.Select(line =>
                {
                    var product = productsById[line.ProductId];
                    return new OrderItemDomain
                    {
                        ProductId = product.Id,
                        ProductName = product.Title ?? product.Name,
                        Quantity = line.Quantity,
                        UnitPrice = product.Price
                    };
                }).ToList()
            };

            order.Subtotal = order.Items.Sum(item => item.UnitPrice * item.Quantity);

            var created = await _orderRepository.CreateAsync(order, cancellationToken);
            await _actionLogService.RecordAsync(
                "Create",
                nameof(OrderDomain),
                created.Id,
                $"Created order {created.Id} with subtotal {created.Subtotal:0.00}.",
                cancellationToken);

            return _mapper.Map<OrderDto>(created);
        }

        public async Task<OrderDto> PayAsync(int id, CancellationToken cancellationToken)
        {
            var order = EnsureFound(await _orderRepository.GetTrackedByIdAsync(id, cancellationToken), "Order", id);
            EnsureCanAccess(order);

            if (order.Status != OrderStatus.Pending)
            {
                throw new ValidationException($"Only pending orders can be paid. Current status: {order.Status}.");
            }

            var payer = EnsureFound(await _userRepository.GetByIdAsync(order.UserId, cancellationToken), "User", order.UserId);
            if (payer.Balance < order.Subtotal)
            {
                throw new ValidationException("Insufficient balance to pay for this order.");
            }

            var productIds = order.Items.Select(item => item.ProductId).Distinct().ToArray();
            var trackedProducts = await _productRepository.GetTrackedByIdsAsync(productIds, cancellationToken);
            if (trackedProducts.Count != productIds.Length)
            {
                throw new ValidationException("One or more ordered products no longer exist.");
            }

            var trackedProductsById = trackedProducts.ToDictionary(product => product.Id);
            foreach (var item in order.Items)
            {
                var product = trackedProductsById[item.ProductId];
                if (product.IsPreorder)
                {
                    continue;
                }

                if (product.StockQuantity < item.Quantity)
                {
                    throw new ValidationException(
                        $"Not enough stock for '{product.Title ?? product.Name}': available {product.StockQuantity}, required {item.Quantity}.");
                }

                product.StockQuantity -= item.Quantity;
            }

            await using var transaction = await _orderRepository.BeginTransactionAsync(cancellationToken);

            await _userRepository.AdjustBalanceAsync(order.UserId, -order.Subtotal, cancellationToken);
            order.Status = OrderStatus.Paid;
            order.PaidAt = DateTime.UtcNow;
            var updated = await _orderRepository.UpdateAsync(order, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            await _actionLogService.RecordAsync(
                "Pay",
                nameof(OrderDomain),
                updated.Id,
                $"Paid order {updated.Id} for {updated.Subtotal:0.00}.",
                cancellationToken);

            return _mapper.Map<OrderDto>(updated);
        }

        public async Task<OrderDto> CancelAsync(int id, CancellationToken cancellationToken)
        {
            var order = EnsureFound(await _orderRepository.GetTrackedByIdAsync(id, cancellationToken), "Order", id);
            EnsureCanAccess(order);

            if (order.Status is OrderStatus.Shipped or OrderStatus.Completed or OrderStatus.Cancelled)
            {
                throw new ValidationException($"Order in status {order.Status} cannot be cancelled.");
            }

            await using var transaction = await _orderRepository.BeginTransactionAsync(cancellationToken);

            if (order.Status == OrderStatus.Paid)
            {
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

                await _userRepository.AdjustBalanceAsync(order.UserId, order.Subtotal, cancellationToken);
            }

            order.Status = OrderStatus.Cancelled;
            var updated = await _orderRepository.UpdateAsync(order, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            await _actionLogService.RecordAsync(
                "Cancel",
                nameof(OrderDomain),
                updated.Id,
                $"Cancelled order {updated.Id}.",
                cancellationToken);

            return _mapper.Map<OrderDto>(updated);
        }

        private async Task<PagedResult<OrderDto>> ListAsync(
            OrderListRequestDto request,
            int? userId,
            CancellationToken cancellationToken)
        {
            var queryOptions = new OrderQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat", "subtotal", "status"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                Status = request.Status,
                UserId = userId
            };

            var orders = await _orderRepository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<OrderDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<OrderDto>>(orders.Items),
                TotalCount = orders.TotalCount,
                Page = orders.Page,
                PageSize = orders.PageSize
            };
        }

        private int RequireUserId()
        {
            return _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");
        }

        private void EnsureCanAccess(OrderDomain order)
        {
            var userId = RequireUserId();
            if (order.UserId == userId || _currentUserContext.Role == UserRole.Admin)
            {
                return;
            }

            throw new UnauthorizedAppException("You do not have access to this order.");
        }
    }
}
