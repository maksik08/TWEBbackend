using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IWarehouseOperationsService
    {
        Task<IReadOnlyCollection<WarehouseZoneDto>> GetZonesAsync(CancellationToken cancellationToken);
        Task<WarehouseZoneDto> CreateZoneAsync(CreateWarehouseZoneDto dto, CancellationToken cancellationToken);
        Task<ProductDto> UpdateThresholdsAsync(int productId, UpdateStockThresholdsDto dto, CancellationToken cancellationToken);
        Task<InventoryCountDto> RunInventoryCountAsync(CreateInventoryCountDto dto, CancellationToken cancellationToken);
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto, CancellationToken cancellationToken);
        Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(ListQueryRequestDto request, CancellationToken cancellationToken);
        Task<GoodsReceiptDto> ReceiveGoodsAsync(CreateGoodsReceiptDto dto, CancellationToken cancellationToken);
        Task<SupplierReturnDto> CreateSupplierReturnAsync(CreateSupplierReturnDto dto, CancellationToken cancellationToken);
        Task<PagedResult<SupplierReturnDto>> GetSupplierReturnsAsync(ListQueryRequestDto request, CancellationToken cancellationToken);
        Task<WarehouseTransferDto> TransferAsync(CreateWarehouseTransferDto dto, CancellationToken cancellationToken);
        Task<PagedResult<WarehouseDocumentDto>> GetDocumentsAsync(ListQueryRequestDto request, WarehouseDocumentType? type, CancellationToken cancellationToken);
        Task<WarehouseDocumentDto> PrintDocumentAsync(WarehouseDocumentType type, int relatedEntityId, CancellationToken cancellationToken);
        Task<StockForecastDto> ForecastAsync(CancellationToken cancellationToken);
        Task ReserveForOrderAsync(OrderDomain order, CancellationToken cancellationToken);
        Task ConsumeReservationsAsync(int orderId, CancellationToken cancellationToken);
        Task ReleaseReservationsAsync(int orderId, CancellationToken cancellationToken);
    }
}
