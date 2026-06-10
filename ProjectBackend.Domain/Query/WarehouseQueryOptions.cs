using ProjectBackend.Domain.Entities;

namespace ProjectBackend.Domain.Query
{
    public sealed class PurchaseOrderQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }
        public PurchaseOrderStatus? Status { get; init; }
        public int? SupplierId { get; init; }
    }

    public sealed class SupplierReturnQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }
        public SupplierReturnStatus? Status { get; init; }
        public int? SupplierId { get; init; }
    }

    public sealed class WarehouseDocumentQueryOptions : PagedQueryOptions
    {
        public WarehouseDocumentType? Type { get; init; }
    }
}
