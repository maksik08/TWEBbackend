namespace ProjectBackend.Domain.Entities
{
    public enum WarehouseMovementType
    {
        InventoryCount = 0,
        Reservation = 1,
        ReservationRelease = 2,
        Sale = 3,
        GoodsReceipt = 4,
        SupplierReturn = 5,
        TransferOut = 6,
        TransferIn = 7,
        Adjustment = 8
    }
}
