namespace ProjectBackend.BusinessLogic.Services
{
    /// <summary>
    /// In-memory stand-in for an external warehouse. Returns pseudo-random stock levels so
    /// repeated syncs show movement, and occasionally omits a SKU to simulate items the
    /// warehouse does not track. Replace with a real provider to integrate for real.
    /// </summary>
    public class MockWarehouseStockProvider : IWarehouseStockProvider
    {
        public Task<IReadOnlyCollection<WarehouseStockItem>> GetStockLevelsAsync(
            IReadOnlyCollection<string> skus,
            CancellationToken cancellationToken)
        {
            var result = new List<WarehouseStockItem>(skus.Count);

            foreach (var sku in skus.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(sku))
                {
                    continue;
                }

                // ~10% of SKUs are "not tracked" by the warehouse.
                if (Random.Shared.Next(0, 10) == 0)
                {
                    continue;
                }

                result.Add(new WarehouseStockItem(sku, Random.Shared.Next(0, 120)));
            }

            return Task.FromResult<IReadOnlyCollection<WarehouseStockItem>>(result);
        }
    }
}
