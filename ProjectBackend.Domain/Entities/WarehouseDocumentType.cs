namespace ProjectBackend.Domain.Entities
{
    public enum WarehouseDocumentType
    {
        InventoryCount = 0,
        PurchaseOrder = 1,
        SupplierReturn = 2,
        GoodsReceipt = 3,
        Transfer = 4,
        Forecast = 5
    }
}
