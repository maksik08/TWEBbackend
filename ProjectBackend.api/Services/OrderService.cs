using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
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
        private readonly ProjectDbContext _dbContext;
        private readonly ICurrentUserContext _currentUser;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            ProjectDbContext dbContext,
            ICurrentUserContext currentUser,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _dbContext = dbContext;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public Task<PagedResult<OrderDto>> GetMineAsync(OrderListRequestDto request, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();
            return ListAsync(request, userId, cancellationToken);
        }

        public Task<PagedResult<OrderDto>> GetAllAsync(OrderListRequestDto request, CancellationToken cancellationToken)
        {
            return ListAsync(request, request.UserId, cancellationToken);
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

            if (dto.Items is null || dto.Items.Count == 0)
            {
                throw new ValidationException("Order must contain at least one item.");
            }

            var aggregated = dto.Items
                .GroupBy(i => i.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Quantity = group.Sum(g => g.Quantity)
                })
                .ToList();

            foreach (var line in aggregated)
            {
                if (line.Quantity <= 0)
                {
                    throw new ValidationException($"Quantity for product {line.ProductId} must be greater than zero.");
                }
            }

            var productIds = aggregated.Select(line => line.ProductId).ToList();
            var products = await _dbContext.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            var missing = productIds.Where(id => !products.ContainsKey(id)).ToList();
            if (missing.Count > 0)
            {
                throw new ValidationException($"Products not found: {string.Join(", ", missing)}.");
            }

            await using var transaction = await _orderRepository.BeginTransactionAsync(cancellationToken);

            var order = new OrderDomain
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                Items = aggregated.Select(line =>
                {
                    var product = products[line.ProductId];
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
            await transaction.CommitAsync(cancellationToken);

            var loaded = await _orderRepository.GetByIdAsync(created.Id, cancellationToken);
            return _mapper.Map<OrderDto>(loaded!);
        }

        public async Task<OrderDto> PayAsync(int id, CancellationToken cancellationToken)
        {
            var order = EnsureFound(await _orderRepository.GetByIdAsync(id, cancellationToken), "Order", id);
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

            await using var transaction = await _orderRepository.BeginTransactionAsync(cancellationToken);

            await _userRepository.AdjustBalanceAsync(order.UserId, -order.Subtotal, cancellationToken);
            var updated = await _orderRepository.UpdateStatusAsync(id, OrderStatus.Paid, DateTime.UtcNow, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return _mapper.Map<OrderDto>(updated!);
        }

        public async Task<OrderDto> CancelAsync(int id, CancellationToken cancellationToken)
        {
            var order = EnsureFound(await _orderRepository.GetByIdAsync(id, cancellationToken), "Order", id);
            EnsureCanAccess(order);

            if (order.Status is OrderStatus.Shipped or OrderStatus.Completed or OrderStatus.Cancelled)
            {
                throw new ValidationException($"Order in status {order.Status} cannot be cancelled.");
            }

            await using var transaction = await _orderRepository.BeginTransactionAsync(cancellationToken);

            if (order.Status == OrderStatus.Paid)
            {
                await _userRepository.AdjustBalanceAsync(order.UserId, order.Subtotal, cancellationToken);
            }

            var updated = await _orderRepository.UpdateStatusAsync(id, OrderStatus.Cancelled, null, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return _mapper.Map<OrderDto>(updated!);
        }

        private async Task<PagedResult<OrderDto>> ListAsync(OrderListRequestDto request, int? userId, CancellationToken cancellationToken)
        {
            var queryOptions = new OrderQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat", "subtotal", "status"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
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
            return _currentUser.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");
        }

        private void EnsureCanAccess(OrderDomain order)
        {
            var userId = RequireUserId();
            if (order.UserId == userId) return;
            if (_currentUser.Role == UserRole.Admin) return;

            throw new UnauthorizedAppException("You don't have access to this order.");
        }
    }
}
