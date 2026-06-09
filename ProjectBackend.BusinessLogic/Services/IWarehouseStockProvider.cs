namespace ProjectBackend.BusinessLogic.Services
{
    /// <summary>
    /// A single stock level reported by the external warehouse for a SKU.
    /// </summary>
    public sealed record WarehouseStockItem(string Sku, int Quantity);

    /// <summary>
    /// Abstraction over an external warehouse-management system.
    /// Swap <see cref="MockWarehouseStockProvider"/> for a real HTTP client to integrate for real.
    /// </summary>
    public interface IWarehouseStockProvider
    {
        /// <summary>
        /// Returns current stock levels for the requested SKUs. SKUs the warehouse does not
        /// track are simply omitted from the result.
        /// </summary>
        Task<IReadOnlyCollection<WarehouseStockItem>> GetStockLevelsAsync(
            IReadOnlyCollection<string> skus,
            CancellationToken cancellationToken);
    }
}
