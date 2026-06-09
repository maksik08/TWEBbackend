using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class WarehouseSyncService : IWarehouseSyncService
    {
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseStockProvider _stockProvider;
        private readonly IActionLogService _actionLogService;

        public WarehouseSyncService(
            IProductRepository productRepository,
            IWarehouseStockProvider stockProvider,
            IActionLogService actionLogService)
        {
            _productRepository = productRepository;
            _stockProvider = stockProvider;
            _actionLogService = actionLogService;
        }

        public async Task<WarehouseSyncResultDto> SyncAsync(bool dryRun, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllForStockReportAsync(cancellationToken);

            var withSku = products
                .Where(product => !string.IsNullOrWhiteSpace(product.Sku))
                .ToList();

            var skus = withSku
                .Select(product => product.Sku!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var warehouseLevels = await _stockProvider.GetStockLevelsAsync(skus, cancellationToken);
            var quantityBySku = warehouseLevels
                .GroupBy(item => item.Sku, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Last().Quantity, StringComparer.OrdinalIgnoreCase);

            var result = new WarehouseSyncResultDto
            {
                Applied = !dryRun,
                TotalProducts = products.Count,
                WithoutSku = products.Count - withSku.Count,
                SyncedAt = DateTime.UtcNow
            };

            var changes = new List<WarehouseSyncLineDto>();
            var updates = new Dictionary<int, int>();

            foreach (var product in withSku)
            {
                if (!quantityBySku.TryGetValue(product.Sku!, out var newQuantity))
                {
                    result.NotInWarehouse++;
                    continue;
                }

                if (newQuantity == product.StockQuantity)
                {
                    result.Unchanged++;
                    continue;
                }

                changes.Add(new WarehouseSyncLineDto
                {
                    ProductId = product.Id,
                    Sku = product.Sku!,
                    ProductName = product.Title ?? product.Name,
                    PreviousQuantity = product.StockQuantity,
                    NewQuantity = newQuantity
                });
                updates[product.Id] = newQuantity;
            }

            result.Updated = changes.Count;
            result.Changes = changes;

            if (!dryRun && updates.Count > 0)
            {
                await _productRepository.UpdateStockLevelsAsync(updates, cancellationToken);

                await _actionLogService.RecordAsync(
                    "WarehouseSync",
                    "Warehouse",
                    null,
                    $"Synced warehouse stock: {result.Updated} updated, {result.Unchanged} unchanged, {result.NotInWarehouse} not tracked, {result.WithoutSku} without SKU.",
                    cancellationToken);
            }

            return result;
        }
    }
}
