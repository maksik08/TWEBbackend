using Microsoft.EntityFrameworkCore;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ProjectDbContext _dbContext;

        public WarehouseRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<WarehouseZoneDomain>> GetZonesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.WarehouseZones
                .AsNoTracking()
                .Include(zone => zone.Warehouse)
                .OrderBy(zone => zone.Warehouse!.Name)
                .ThenBy(zone => zone.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<WarehouseZoneDomain?> GetZoneAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.WarehouseZones
                .Include(zone => zone.Warehouse)
                .FirstOrDefaultAsync(zone => zone.Id == id, cancellationToken);
        }

        public async Task<WarehouseZoneDomain> CreateZoneAsync(WarehouseZoneDomain zone, CancellationToken cancellationToken)
        {
            await EnsureDefaultWarehouseAsync(cancellationToken);
            await _dbContext.WarehouseZones.AddAsync(zone, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(zone).Reference(item => item.Warehouse).LoadAsync(cancellationToken);
            return zone;
        }

        public async Task<ProductsDomain?> GetTrackedProductAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<ProductsDomain>> GetTrackedProductsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .Where(product => ids.Contains(product.Id))
                .ToListAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<InventoryCountDomain> CreateInventoryCountAsync(InventoryCountDomain inventoryCount, CancellationToken cancellationToken)
        {
            await _dbContext.InventoryCounts.AddAsync(inventoryCount, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return await LoadInventoryCountAsync(inventoryCount.Id, cancellationToken) ?? inventoryCount;
        }

        public async Task<PurchaseOrderDomain> CreatePurchaseOrderAsync(PurchaseOrderDomain purchaseOrder, CancellationToken cancellationToken)
        {
            await _dbContext.PurchaseOrders.AddAsync(purchaseOrder, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return await LoadPurchaseOrderAsync(purchaseOrder.Id, cancellationToken) ?? purchaseOrder;
        }

        public async Task<PagedResult<PurchaseOrderDomain>> GetPurchaseOrdersAsync(PurchaseOrderQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.PurchaseOrders
                .AsNoTracking()
                .Include(order => order.Supplier)
                .Include(order => order.Items)
                    .ThenInclude(item => item.Product)
                .AsQueryable();

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(order => order.Status == queryOptions.Status.Value);
            }

            if (queryOptions.SupplierId.HasValue)
            {
                query = query.Where(order => order.SupplierId == queryOptions.SupplierId.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(order => order.OrderNumber.Contains(queryOptions.Search));
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(order => order.CreatedAt).ThenByDescending(order => order.Id)
                : query.OrderBy(order => order.CreatedAt).ThenBy(order => order.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<PurchaseOrderDomain?> GetTrackedPurchaseOrderAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.PurchaseOrders
                .Include(order => order.Items)
                .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
        }

        public async Task<GoodsReceiptDomain> CreateGoodsReceiptAsync(GoodsReceiptDomain receipt, CancellationToken cancellationToken)
        {
            await _dbContext.GoodsReceipts.AddAsync(receipt, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return await _dbContext.GoodsReceipts
                .AsNoTracking()
                .Include(item => item.Supplier)
                .Include(item => item.Items)
                    .ThenInclude(item => item.Product)
                .FirstAsync(item => item.Id == receipt.Id, cancellationToken);
        }

        public async Task<SupplierReturnDomain> CreateSupplierReturnAsync(SupplierReturnDomain supplierReturn, CancellationToken cancellationToken)
        {
            await _dbContext.SupplierReturns.AddAsync(supplierReturn, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return await LoadSupplierReturnAsync(supplierReturn.Id, cancellationToken) ?? supplierReturn;
        }

        public async Task<PagedResult<SupplierReturnDomain>> GetSupplierReturnsAsync(SupplierReturnQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.SupplierReturns
                .AsNoTracking()
                .Include(item => item.Supplier)
                .Include(item => item.Items)
                    .ThenInclude(item => item.Product)
                .AsQueryable();

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(item => item.Status == queryOptions.Status.Value);
            }

            if (queryOptions.SupplierId.HasValue)
            {
                query = query.Where(item => item.SupplierId == queryOptions.SupplierId.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(item => item.ReturnNumber.Contains(queryOptions.Search) || item.Reason.Contains(queryOptions.Search));
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(item => item.CreatedAt).ThenByDescending(item => item.Id)
                : query.OrderBy(item => item.CreatedAt).ThenBy(item => item.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<WarehouseTransferDomain> CreateTransferAsync(WarehouseTransferDomain transfer, CancellationToken cancellationToken)
        {
            await _dbContext.WarehouseTransfers.AddAsync(transfer, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(transfer).Reference(item => item.Product).LoadAsync(cancellationToken);
            await _dbContext.Entry(transfer).Reference(item => item.FromZone).LoadAsync(cancellationToken);
            await _dbContext.Entry(transfer).Reference(item => item.ToZone).LoadAsync(cancellationToken);
            return transfer;
        }

        public async Task<WarehouseDocumentDomain> CreateDocumentAsync(WarehouseDocumentDomain document, CancellationToken cancellationToken)
        {
            await _dbContext.WarehouseDocuments.AddAsync(document, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return document;
        }

        public async Task<PagedResult<WarehouseDocumentDomain>> GetDocumentsAsync(WarehouseDocumentQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.WarehouseDocuments.AsNoTracking().AsQueryable();

            if (queryOptions.Type.HasValue)
            {
                query = query.Where(document => document.Type == queryOptions.Type.Value);
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(document => document.CreatedAt).ThenByDescending(document => document.Id)
                : query.OrderBy(document => document.CreatedAt).ThenBy(document => document.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<ProductStockReservationDomain> AddReservationAsync(ProductStockReservationDomain reservation, CancellationToken cancellationToken)
        {
            await _dbContext.ProductStockReservations.AddAsync(reservation, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return reservation;
        }

        public async Task<IReadOnlyCollection<ProductStockReservationDomain>> GetTrackedReservationsForOrderAsync(int orderId, CancellationToken cancellationToken)
        {
            return await _dbContext.ProductStockReservations
                .Include(reservation => reservation.Product)
                .Where(reservation => reservation.OrderId == orderId && reservation.Status == ProductStockReservationStatus.Reserved)
                .ToListAsync(cancellationToken);
        }

        public async Task AddMovementAsync(StockMovementDomain movement, CancellationToken cancellationToken)
        {
            await _dbContext.StockMovements.AddAsync(movement, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task EnsureDefaultWarehouseAsync(CancellationToken cancellationToken)
        {
            if (await _dbContext.Warehouses.AnyAsync(cancellationToken))
            {
                return;
            }

            await _dbContext.Warehouses.AddAsync(new WarehouseDomain
            {
                Name = "Main warehouse",
                Code = "MAIN"
            }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<InventoryCountDomain?> LoadInventoryCountAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.InventoryCounts
                .AsNoTracking()
                .Include(count => count.WarehouseZone)
                .Include(count => count.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(count => count.Id == id, cancellationToken);
        }

        private async Task<PurchaseOrderDomain?> LoadPurchaseOrderAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.PurchaseOrders
                .AsNoTracking()
                .Include(order => order.Supplier)
                .Include(order => order.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
        }

        private async Task<SupplierReturnDomain?> LoadSupplierReturnAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.SupplierReturns
                .AsNoTracking()
                .Include(item => item.Supplier)
                .Include(item => item.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        }
    }
}
