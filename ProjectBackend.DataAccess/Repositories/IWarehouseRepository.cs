using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IWarehouseRepository
    {
        Task<IReadOnlyCollection<WarehouseZoneDomain>> GetZonesAsync(CancellationToken cancellationToken);
        Task<WarehouseZoneDomain?> GetZoneAsync(int id, CancellationToken cancellationToken);
        Task<WarehouseZoneDomain> CreateZoneAsync(WarehouseZoneDomain zone, CancellationToken cancellationToken);
        Task<ProductsDomain?> GetTrackedProductAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<ProductsDomain>> GetTrackedProductsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<InventoryCountDomain> CreateInventoryCountAsync(InventoryCountDomain inventoryCount, CancellationToken cancellationToken);
        Task<PurchaseOrderDomain> CreatePurchaseOrderAsync(PurchaseOrderDomain purchaseOrder, CancellationToken cancellationToken);
        Task<PagedResult<PurchaseOrderDomain>> GetPurchaseOrdersAsync(PurchaseOrderQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<PurchaseOrderDomain?> GetTrackedPurchaseOrderAsync(int id, CancellationToken cancellationToken);
        Task<GoodsReceiptDomain> CreateGoodsReceiptAsync(GoodsReceiptDomain receipt, CancellationToken cancellationToken);
        Task<SupplierReturnDomain> CreateSupplierReturnAsync(SupplierReturnDomain supplierReturn, CancellationToken cancellationToken);
        Task<PagedResult<SupplierReturnDomain>> GetSupplierReturnsAsync(SupplierReturnQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<WarehouseTransferDomain> CreateTransferAsync(WarehouseTransferDomain transfer, CancellationToken cancellationToken);
        Task<WarehouseDocumentDomain> CreateDocumentAsync(WarehouseDocumentDomain document, CancellationToken cancellationToken);
        Task<PagedResult<WarehouseDocumentDomain>> GetDocumentsAsync(WarehouseDocumentQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<ProductStockReservationDomain> AddReservationAsync(ProductStockReservationDomain reservation, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<ProductStockReservationDomain>> GetTrackedReservationsForOrderAsync(int orderId, CancellationToken cancellationToken);
        Task AddMovementAsync(StockMovementDomain movement, CancellationToken cancellationToken);
    }
}
