using AutoMapper;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.DataAccess.Repositories;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.BusinessLogic.Services
{
    public class WarehouseOperationsService : ApplicationServiceBase, IWarehouseOperationsService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IActionLogService _actionLogService;
        private readonly IMapper _mapper;

        public WarehouseOperationsService(
            IWarehouseRepository warehouseRepository,
            IProductRepository productRepository,
            ISupplierRepository supplierRepository,
            INotificationService notificationService,
            IUserRepository userRepository,
            IActionLogService actionLogService,
            IMapper mapper)
        {
            _warehouseRepository = warehouseRepository;
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _actionLogService = actionLogService;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<WarehouseZoneDto>> GetZonesAsync(CancellationToken cancellationToken)
        {
            var zones = await _warehouseRepository.GetZonesAsync(cancellationToken);
            return _mapper.Map<IReadOnlyCollection<WarehouseZoneDto>>(zones);
        }

        public async Task<WarehouseZoneDto> CreateZoneAsync(CreateWarehouseZoneDto dto, CancellationToken cancellationToken)
        {
            var zone = await _warehouseRepository.CreateZoneAsync(new WarehouseZoneDomain
            {
                WarehouseId = dto.WarehouseId,
                Name = NormalizeRequiredText(dto.Name, "Zone name"),
                Code = NormalizeRequiredText(dto.Code, "Zone code").ToUpperInvariant(),
                Description = NormalizeOptionalText(dto.Description),
                IsActive = true
            }, cancellationToken);

            return _mapper.Map<WarehouseZoneDto>(zone);
        }

        public async Task<ProductDto> UpdateThresholdsAsync(int productId, UpdateStockThresholdsDto dto, CancellationToken cancellationToken)
        {
            ValidateThresholds(dto.MinStockLevel, dto.MaxStockLevel);
            var product = EnsureFound(await _warehouseRepository.GetTrackedProductAsync(productId, cancellationToken), "Product", productId);
            product.MinStockLevel = dto.MinStockLevel;
            product.MaxStockLevel = dto.MaxStockLevel;
            await _warehouseRepository.SaveChangesAsync(cancellationToken);
            await NotifyLowStockAsync(product, cancellationToken);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<InventoryCountDto> RunInventoryCountAsync(CreateInventoryCountDto dto, CancellationToken cancellationToken)
        {
            var productIds = dto.Items.Select(item => item.ProductId).Distinct().ToArray();
            var products = await _warehouseRepository.GetTrackedProductsAsync(productIds, cancellationToken);
            if (products.Count != productIds.Length)
            {
                throw new ValidationException("One or more products do not exist.");
            }

            var productsById = products.ToDictionary(product => product.Id);
            var count = new InventoryCountDomain
            {
                CountNumber = NextNumber("IC"),
                WarehouseZoneId = dto.WarehouseZoneId,
                Note = NormalizeOptionalText(dto.Note),
                Items = dto.Items.Select(item =>
                {
                    var product = productsById[item.ProductId];
                    return new InventoryCountItemDomain
                    {
                        ProductId = item.ProductId,
                        SystemQuantity = product.StockQuantity,
                        CountedQuantity = item.CountedQuantity,
                        Variance = item.CountedQuantity - product.StockQuantity
                    };
                }).ToList()
            };

            foreach (var item in count.Items)
            {
                var product = productsById[item.ProductId];
                product.StockQuantity = item.CountedQuantity;
                product.ReservedQuantity = Math.Min(product.ReservedQuantity, product.StockQuantity);
                await AddMovementAsync(product, WarehouseMovementType.InventoryCount, item.Variance, nameof(InventoryCountDomain), null, "Inventory count adjustment.", cancellationToken);
            }

            var created = await _warehouseRepository.CreateInventoryCountAsync(count, cancellationToken);
            await _actionLogService.RecordAsync("InventoryCount", nameof(InventoryCountDomain), created.Id, $"Inventory count {created.CountNumber} applied.", cancellationToken);
            return _mapper.Map<InventoryCountDto>(created);
        }

        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto, CancellationToken cancellationToken)
        {
            if (!await _supplierRepository.ExistsAsync(dto.SupplierId, cancellationToken))
            {
                throw new ValidationException("Supplier does not exist.");
            }

            var products = await _warehouseRepository.GetTrackedProductsAsync(dto.Items.Select(item => item.ProductId).Distinct().ToArray(), cancellationToken);
            if (products.Count != dto.Items.Select(item => item.ProductId).Distinct().Count())
            {
                throw new ValidationException("One or more products do not exist.");
            }

            var order = await _warehouseRepository.CreatePurchaseOrderAsync(new PurchaseOrderDomain
            {
                OrderNumber = NextNumber("PO"),
                SupplierId = dto.SupplierId,
                ExpectedAt = dto.ExpectedAt,
                OrderedAt = DateTime.UtcNow,
                Notes = NormalizeOptionalText(dto.Notes),
                Status = PurchaseOrderStatus.Ordered,
                Items = dto.Items.Select(item => new PurchaseOrderItemDomain
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                }).ToList()
            }, cancellationToken);

            return _mapper.Map<PurchaseOrderDto>(order);
        }

        public async Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(ListQueryRequestDto request, CancellationToken cancellationToken)
        {
            var orders = await _warehouseRepository.GetPurchaseOrdersAsync(new PurchaseOrderQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = "createdat",
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection ?? "desc")
            }, cancellationToken);

            return MapPaged<PurchaseOrderDomain, PurchaseOrderDto>(orders);
        }

        public async Task<GoodsReceiptDto> ReceiveGoodsAsync(CreateGoodsReceiptDto dto, CancellationToken cancellationToken)
        {
            if (!dto.QualityCheckPassed || !dto.CompletenessCheckPassed)
            {
                throw new ValidationException("Quality and completeness checks must pass before goods are accepted.");
            }

            var productIds = dto.Items.Select(item => item.ProductId).Distinct().ToArray();
            var products = await _warehouseRepository.GetTrackedProductsAsync(productIds, cancellationToken);
            if (products.Count != productIds.Length)
            {
                throw new ValidationException("One or more products do not exist.");
            }

            var productsById = products.ToDictionary(product => product.Id);
            foreach (var item in dto.Items)
            {
                if (item.AcceptedQuantity + item.RejectedQuantity <= 0)
                {
                    throw new ValidationException("Receipt item must contain accepted or rejected quantity.");
                }

                var product = productsById[item.ProductId];
                product.StockQuantity += item.AcceptedQuantity;
                await AddMovementAsync(product, WarehouseMovementType.GoodsReceipt, item.AcceptedQuantity, nameof(GoodsReceiptDomain), null, "Goods receipt.", cancellationToken);
            }

            var receipt = await _warehouseRepository.CreateGoodsReceiptAsync(new GoodsReceiptDomain
            {
                ReceiptNumber = NextNumber("GR"),
                PurchaseOrderId = dto.PurchaseOrderId,
                SupplierId = dto.SupplierId,
                QualityCheckPassed = dto.QualityCheckPassed,
                CompletenessCheckPassed = dto.CompletenessCheckPassed,
                Status = WarehouseReceiptStatus.Accepted,
                Notes = NormalizeOptionalText(dto.Notes),
                Items = dto.Items.Select(item => new GoodsReceiptItemDomain
                {
                    ProductId = item.ProductId,
                    AcceptedQuantity = item.AcceptedQuantity,
                    RejectedQuantity = item.RejectedQuantity,
                    Note = NormalizeOptionalText(item.Note)
                }).ToList()
            }, cancellationToken);

            return _mapper.Map<GoodsReceiptDto>(receipt);
        }

        public async Task<SupplierReturnDto> CreateSupplierReturnAsync(CreateSupplierReturnDto dto, CancellationToken cancellationToken)
        {
            if (!await _supplierRepository.ExistsAsync(dto.SupplierId, cancellationToken))
            {
                throw new ValidationException("Supplier does not exist.");
            }

            var products = await _warehouseRepository.GetTrackedProductsAsync(dto.Items.Select(item => item.ProductId).Distinct().ToArray(), cancellationToken);
            var productsById = products.ToDictionary(product => product.Id);

            foreach (var item in dto.Items)
            {
                var product = EnsureFound(productsById.GetValueOrDefault(item.ProductId), "Product", item.ProductId);
                if (product.AvailableQuantity < item.Quantity)
                {
                    throw new ValidationException($"Not enough available stock for '{product.Name}'.");
                }

                product.StockQuantity -= item.Quantity;
                await AddMovementAsync(product, WarehouseMovementType.SupplierReturn, -item.Quantity, nameof(SupplierReturnDomain), null, "Supplier return.", cancellationToken);
                await NotifyLowStockAsync(product, cancellationToken);
            }

            var supplierReturn = await _warehouseRepository.CreateSupplierReturnAsync(new SupplierReturnDomain
            {
                ReturnNumber = NextNumber("SR"),
                SupplierId = dto.SupplierId,
                Reason = NormalizeRequiredText(dto.Reason, "Reason"),
                Status = SupplierReturnStatus.Sent,
                SentAt = DateTime.UtcNow,
                Items = dto.Items.Select(item => new SupplierReturnItemDomain
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Reason = NormalizeOptionalText(item.Reason)
                }).ToList()
            }, cancellationToken);

            return _mapper.Map<SupplierReturnDto>(supplierReturn);
        }

        public async Task<PagedResult<SupplierReturnDto>> GetSupplierReturnsAsync(ListQueryRequestDto request, CancellationToken cancellationToken)
        {
            var returns = await _warehouseRepository.GetSupplierReturnsAsync(new SupplierReturnQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = "createdat",
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection ?? "desc")
            }, cancellationToken);

            return MapPaged<SupplierReturnDomain, SupplierReturnDto>(returns);
        }

        public async Task<WarehouseTransferDto> TransferAsync(CreateWarehouseTransferDto dto, CancellationToken cancellationToken)
        {
            var product = EnsureFound(await _warehouseRepository.GetTrackedProductAsync(dto.ProductId, cancellationToken), "Product", dto.ProductId);
            EnsureFound(await _warehouseRepository.GetZoneAsync(dto.ToZoneId, cancellationToken), "Warehouse zone", dto.ToZoneId);

            product.WarehouseZoneId = dto.ToZoneId;
            var transfer = await _warehouseRepository.CreateTransferAsync(new WarehouseTransferDomain
            {
                ProductId = dto.ProductId,
                FromZoneId = dto.FromZoneId,
                ToZoneId = dto.ToZoneId,
                Quantity = dto.Quantity,
                Status = WarehouseTransferStatus.Completed,
                CompletedAt = DateTime.UtcNow,
                Note = NormalizeOptionalText(dto.Note)
            }, cancellationToken);

            await AddMovementAsync(product, WarehouseMovementType.TransferIn, 0, nameof(WarehouseTransferDomain), transfer.Id, "Warehouse zone transfer.", cancellationToken);
            return _mapper.Map<WarehouseTransferDto>(transfer);
        }

        public async Task<PagedResult<WarehouseDocumentDto>> GetDocumentsAsync(ListQueryRequestDto request, WarehouseDocumentType? type, CancellationToken cancellationToken)
        {
            var documents = await _warehouseRepository.GetDocumentsAsync(new WarehouseDocumentQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = "createdat",
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection ?? "desc"),
                Type = type
            }, cancellationToken);

            return MapPaged<WarehouseDocumentDomain, WarehouseDocumentDto>(documents);
        }

        public async Task<WarehouseDocumentDto> PrintDocumentAsync(WarehouseDocumentType type, int relatedEntityId, CancellationToken cancellationToken)
        {
            var document = await _warehouseRepository.CreateDocumentAsync(new WarehouseDocumentDomain
            {
                Type = type,
                Number = NextNumber("DOC"),
                RelatedEntityType = type.ToString(),
                RelatedEntityId = relatedEntityId,
                Title = $"{type} document #{relatedEntityId}",
                Content = $"Document: {type}\nRelated id: {relatedEntityId}\nGenerated at: {DateTime.UtcNow:O}"
            }, cancellationToken);

            return _mapper.Map<WarehouseDocumentDto>(document);
        }

        public async Task<StockForecastDto> ForecastAsync(CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllForStockReportAsync(cancellationToken);
            return new StockForecastDto
            {
                GeneratedAt = DateTime.UtcNow,
                Items = products
                    .Where(product => product.MinStockLevel > 0 && product.AvailableQuantity <= product.MinStockLevel)
                    .Select(product => new StockForecastItemDto
                    {
                        ProductId = product.Id,
                        ProductName = product.Title ?? product.Name,
                        StockQuantity = product.StockQuantity,
                        ReservedQuantity = product.ReservedQuantity,
                        MinStockLevel = product.MinStockLevel,
                        MaxStockLevel = product.MaxStockLevel,
                        RecommendedOrderQuantity = Math.Max((product.MaxStockLevel ?? product.MinStockLevel * 2) - product.AvailableQuantity, 0),
                        Reason = "Available stock is at or below minimum threshold."
                    })
                    .ToList()
            };
        }

        public async Task ReserveForOrderAsync(OrderDomain order, CancellationToken cancellationToken)
        {
            var productIds = order.Items.Select(item => item.ProductId).Distinct().ToArray();
            var products = await _warehouseRepository.GetTrackedProductsAsync(productIds, cancellationToken);
            var productsById = products.ToDictionary(product => product.Id);

            foreach (var item in order.Items)
            {
                var product = productsById[item.ProductId];
                if (product.IsPreorder)
                {
                    continue;
                }

                if (product.AvailableQuantity < item.Quantity)
                {
                    throw new ValidationException($"Not enough available stock for '{product.Title ?? product.Name}'.");
                }

                product.ReservedQuantity += item.Quantity;
                await _warehouseRepository.AddReservationAsync(new ProductStockReservationDomain
                {
                    ProductId = product.Id,
                    OrderId = order.Id,
                    Quantity = item.Quantity,
                    Status = ProductStockReservationStatus.Reserved
                }, cancellationToken);
                await AddMovementAsync(product, WarehouseMovementType.Reservation, 0, nameof(OrderDomain), order.Id, "Reserved stock for order.", cancellationToken);
            }

            await _warehouseRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task ConsumeReservationsAsync(int orderId, CancellationToken cancellationToken)
        {
            var reservations = await _warehouseRepository.GetTrackedReservationsForOrderAsync(orderId, cancellationToken);
            foreach (var reservation in reservations)
            {
                var product = reservation.Product!;
                product.ReservedQuantity = Math.Max(product.ReservedQuantity - reservation.Quantity, 0);
                product.StockQuantity -= reservation.Quantity;
                reservation.Status = ProductStockReservationStatus.Consumed;
                reservation.ReleasedAt = DateTime.UtcNow;
                await AddMovementAsync(product, WarehouseMovementType.Sale, -reservation.Quantity, nameof(OrderDomain), orderId, "Consumed reservation for paid order.", cancellationToken);
                await NotifyLowStockAsync(product, cancellationToken);
            }

            await _warehouseRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task ReleaseReservationsAsync(int orderId, CancellationToken cancellationToken)
        {
            var reservations = await _warehouseRepository.GetTrackedReservationsForOrderAsync(orderId, cancellationToken);
            foreach (var reservation in reservations)
            {
                var product = reservation.Product!;
                product.ReservedQuantity = Math.Max(product.ReservedQuantity - reservation.Quantity, 0);
                reservation.Status = ProductStockReservationStatus.Released;
                reservation.ReleasedAt = DateTime.UtcNow;
                await AddMovementAsync(product, WarehouseMovementType.ReservationRelease, 0, nameof(OrderDomain), orderId, "Released reservation.", cancellationToken);
            }

            await _warehouseRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task AddMovementAsync(ProductsDomain product, WarehouseMovementType type, int quantityDelta, string? referenceType, int? referenceId, string note, CancellationToken cancellationToken)
        {
            await _warehouseRepository.AddMovementAsync(new StockMovementDomain
            {
                ProductId = product.Id,
                WarehouseZoneId = product.WarehouseZoneId,
                Type = type,
                QuantityDelta = quantityDelta,
                BalanceAfter = product.StockQuantity,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                Note = note
            }, cancellationToken);
        }

        private async Task NotifyLowStockAsync(ProductsDomain product, CancellationToken cancellationToken)
        {
            if (product.MinStockLevel <= 0 || product.AvailableQuantity > product.MinStockLevel)
            {
                return;
            }

            var managers = await _userRepository.GetByRoleAsync(UserRole.WarehouseManager, cancellationToken);
            foreach (var manager in managers)
            {
                await _notificationService.NotifyAsync(
                    manager.Id,
                    "Low stock alert",
                    $"{product.Title ?? product.Name} has {product.AvailableQuantity} available units, minimum is {product.MinStockLevel}.",
                    nameof(ProductsDomain),
                    product.Id,
                    cancellationToken);
            }
        }

        private PagedResult<TDto> MapPaged<TEntity, TDto>(PagedResult<TEntity> page)
        {
            return new PagedResult<TDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<TDto>>(page.Items),
                TotalCount = page.TotalCount,
                Page = page.Page,
                PageSize = page.PageSize
            };
        }

        private static void ValidateThresholds(int minStockLevel, int? maxStockLevel)
        {
            EnsureNonNegative(minStockLevel, "Minimum stock level");
            if (maxStockLevel.HasValue && maxStockLevel.Value < minStockLevel)
            {
                throw new ValidationException("Maximum stock level cannot be lower than minimum stock level.");
            }
        }

        private static string NextNumber(string prefix)
        {
            return $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }
}
