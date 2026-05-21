using AutoMapper;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Query;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class ManagerService : ApplicationServiceBase, IManagerService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly INotificationService _notificationService;
        private readonly IActionLogService _actionLogService;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IMapper _mapper;

        public ManagerService(
            IOrderRepository orderRepository,
            IServiceRequestRepository serviceRequestRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            INotificationService notificationService,
            IActionLogService actionLogService,
            ICurrentUserContext currentUserContext,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _notificationService = notificationService;
            _actionLogService = actionLogService;
            _currentUserContext = currentUserContext;
            _mapper = mapper;
        }

        public async Task<PagedResult<OrderDto>> GetOrdersAsync(OrderListRequestDto request, CancellationToken cancellationToken)
        {
            var queryOptions = new OrderQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat", "status", "subtotal"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                Status = request.Status,
                UserId = request.UserId ?? request.CustomerId
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

        public async Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken)
        {
            var order = EnsureFound(await _orderRepository.GetByIdAsync(id, cancellationToken), "Order", id);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken cancellationToken)
        {
            var order = EnsureFound(await _orderRepository.GetTrackedByIdAsync(id, cancellationToken), "Order", id);
            if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled && order.Status != dto.Status)
            {
                throw new ValidationException("Completed or cancelled orders cannot change their status.");
            }

            var previousStatus = order.Status;
            order.Status = dto.Status;
            var updated = await _orderRepository.UpdateAsync(order, cancellationToken);

            await _actionLogService.RecordAsync(
                "UpdateStatus",
                nameof(OrderDomain),
                updated.Id,
                $"Order {updated.Id} status changed from {previousStatus} to {updated.Status}.",
                cancellationToken);

            return _mapper.Map<OrderDto>(updated);
        }

        public async Task<PagedResult<ServiceRequestDto>> GetRequestsAsync(
            ServiceRequestListRequestDto request,
            CancellationToken cancellationToken)
        {
            var queryOptions = new ServiceRequestQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat", "scheduledvisitat", "address"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                Status = request.Status,
                CustomerId = request.CustomerId,
                InstallerId = request.InstallerId,
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

        public async Task<ServiceRequestDto> GetRequestByIdAsync(int id, CancellationToken cancellationToken)
        {
            var request = EnsureFound(await _serviceRequestRepository.GetByIdAsync(id, cancellationToken), "Service request", id);
            return _mapper.Map<ServiceRequestDto>(request);
        }

        public async Task<IReadOnlyCollection<InstallerLookupDto>> GetInstallersAsync(CancellationToken cancellationToken)
        {
            var installers = await _userRepository.GetByRoleAsync(UserRole.Installer, cancellationToken);
            return _mapper.Map<IReadOnlyCollection<InstallerLookupDto>>(installers);
        }

        public async Task<ServiceRequestDto> AssignInstallerAsync(int id, AssignInstallerDto dto, CancellationToken cancellationToken)
        {
            var managerId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var installer = EnsureFound(await _userRepository.GetByIdAsync(dto.InstallerId, cancellationToken), "Installer", dto.InstallerId);
            if (installer.Role != UserRole.Installer)
            {
                throw new ValidationException("The selected user is not an installer.");
            }

            var request = EnsureFound(await _serviceRequestRepository.GetTrackedByIdAsync(id, cancellationToken), "Service request", id);
            if (request.Status is ServiceRequestStatus.Completed or ServiceRequestStatus.Cancelled)
            {
                throw new ValidationException("Completed or cancelled requests cannot be reassigned.");
            }

            request.ManagerId = managerId;
            request.InstallerId = installer.Id;
            request.ScheduledVisitAt = dto.ScheduledVisitAt ?? request.ScheduledVisitAt;
            request.Status = ServiceRequestStatus.Assigned;
            request = await _serviceRequestRepository.UpdateAsync(request, cancellationToken);

            await _notificationService.NotifyAsync(
                installer.Id,
                "New installation request assigned",
                $"Request {request.RequestNumber} has been assigned to you.",
                nameof(ServiceRequestDomain),
                request.Id,
                cancellationToken);

            await _actionLogService.RecordAsync(
                "AssignInstaller",
                nameof(ServiceRequestDomain),
                request.Id,
                $"Assigned installer {installer.Username} to request {request.RequestNumber}.",
                cancellationToken);

            return _mapper.Map<ServiceRequestDto>(request);
        }

        public async Task<ServiceRequestCommentDto> AddCommentAsync(
            int id,
            AddServiceRequestCommentDto dto,
            CancellationToken cancellationToken)
        {
            var managerId = _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");

            var request = EnsureFound(await _serviceRequestRepository.GetTrackedByIdAsync(id, cancellationToken), "Service request", id);
            request.ManagerId ??= managerId;

            var comment = new ServiceRequestCommentDomain
            {
                ServiceRequestId = request.Id,
                AuthorUserId = managerId,
                Message = NormalizeRequiredText(dto.Message, "Comment")
            };

            request.Comments.Add(comment);
            request = await _serviceRequestRepository.UpdateAsync(request, cancellationToken);

            if (request.CustomerId != managerId)
            {
                await _notificationService.NotifyAsync(
                    request.CustomerId,
                    "Service request updated",
                    $"Manager added a comment to request {request.RequestNumber}.",
                    nameof(ServiceRequestDomain),
                    request.Id,
                    cancellationToken);
            }

            await _actionLogService.RecordAsync(
                "AddComment",
                nameof(ServiceRequestDomain),
                request.Id,
                $"Added manager comment to request {request.RequestNumber}.",
                cancellationToken);

            return _mapper.Map<ServiceRequestCommentDto>(request.Comments.OrderByDescending(item => item.Id).First());
        }

        public async Task<SalesReportDto> GetSalesReportAsync(CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetAllForReportAsync(cancellationToken);
            return new SalesReportDto
            {
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(order => order.Subtotal),
                Statuses = orders
                    .GroupBy(order => order.Status)
                    .Select(group => new SalesReportStatusItemDto
                    {
                        Status = group.Key,
                        Count = group.Count()
                    })
                    .OrderBy(item => item.Status)
                    .ToList()
            };
        }

        public async Task<ServiceRequestReportDto> GetServiceRequestReportAsync(CancellationToken cancellationToken)
        {
            var requests = await _serviceRequestRepository.GetAllForReportAsync(cancellationToken);
            return new ServiceRequestReportDto
            {
                TotalRequests = requests.Count,
                AssignedRequests = requests.Count(request => request.Status == ServiceRequestStatus.Assigned),
                CompletedRequests = requests.Count(request => request.Status == ServiceRequestStatus.Completed),
                Statuses = requests
                    .GroupBy(request => request.Status)
                    .Select(group => new ServiceRequestStatusReportItemDto
                    {
                        Status = group.Key,
                        Count = group.Count()
                    })
                    .OrderBy(item => item.Status)
                    .ToList()
            };
        }

        public async Task<StockReportDto> GetStockReportAsync(CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllForStockReportAsync(cancellationToken);
            return new StockReportDto
            {
                TotalProducts = products.Count,
                VisibleProducts = products.Count(product => product.IsVisible),
                TotalUnitsInStock = products.Sum(product => product.StockQuantity),
                Items = products.Select(product => new StockReportItemDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    IsVisible = product.IsVisible
                }).ToList()
            };
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductListRequestDto request, CancellationToken cancellationToken)
        {
            if (request.MinPrice.HasValue && request.MaxPrice.HasValue && request.MinPrice > request.MaxPrice)
            {
                throw new ValidationException("MinPrice cannot be greater than MaxPrice.");
            }

            var queryOptions = new ProductQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "name", "name", "title", "price"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                CategoryId = request.CategoryId,
                SupplierId = request.SupplierId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                IncludeHidden = true
            };

            var products = await _productRepository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<ProductDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<ProductDto>>(products.Items),
                TotalCount = products.TotalCount,
                Page = products.Page,
                PageSize = products.PageSize
            };
        }

        public async Task<ProductDto> UpdateProductVisibilityAsync(
            int id,
            UpdateProductVisibilityDto dto,
            CancellationToken cancellationToken)
        {
            var product = EnsureFound(await _productRepository.SetVisibilityAsync(id, dto.IsVisible, cancellationToken), "Product", id);
            await _actionLogService.RecordAsync(
                "UpdateVisibility",
                nameof(ProductsDomain),
                product.Id,
                $"Set product visibility to {product.IsVisible} for '{product.Name}'.",
                cancellationToken);

            return _mapper.Map<ProductDto>(product);
        }
    }
}
