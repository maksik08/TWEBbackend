using System.ComponentModel.DataAnnotations;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class WarehouseZoneDto : AuditableDto
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateWarehouseZoneDto
    {
        [Range(1, int.MaxValue)]
        public int WarehouseId { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(40)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }
    }

    public class UpdateStockThresholdsDto
    {
        [Range(0, int.MaxValue)]
        public int MinStockLevel { get; set; }

        [Range(0, int.MaxValue)]
        public int? MaxStockLevel { get; set; }
    }

    public class CreateInventoryCountItemDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(0, int.MaxValue)]
        public int CountedQuantity { get; set; }
    }

    public class CreateInventoryCountDto
    {
        public int? WarehouseZoneId { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        [MinLength(1)]
        public List<CreateInventoryCountItemDto> Items { get; set; } = new();
    }

    public class InventoryCountItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int SystemQuantity { get; set; }
        public int CountedQuantity { get; set; }
        public int Variance { get; set; }
    }

    public class InventoryCountDto : AuditableDto
    {
        public int Id { get; set; }
        public string CountNumber { get; set; } = string.Empty;
        public int? WarehouseZoneId { get; set; }
        public string? WarehouseZoneName { get; set; }
        public DateTime CountedAt { get; set; }
        public string? Note { get; set; }
        public IReadOnlyCollection<InventoryCountItemDto> Items { get; set; } = new List<InventoryCountItemDto>();
    }

    public class CreatePurchaseOrderItemDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }
    }

    public class CreatePurchaseOrderDto
    {
        [Range(1, int.MaxValue)]
        public int SupplierId { get; set; }

        public DateTime? ExpectedAt { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MinLength(1)]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class PurchaseOrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal UnitCost { get; set; }
    }

    public class PurchaseOrderDto : AuditableDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public DateTime? ExpectedAt { get; set; }
        public DateTime? OrderedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public string? Notes { get; set; }
        public IReadOnlyCollection<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
    }

    public class CreateGoodsReceiptItemDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(0, int.MaxValue)]
        public int AcceptedQuantity { get; set; }

        [Range(0, int.MaxValue)]
        public int RejectedQuantity { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }

    public class CreateGoodsReceiptDto
    {
        public int? PurchaseOrderId { get; set; }

        public int? SupplierId { get; set; }

        public bool QualityCheckPassed { get; set; }

        public bool CompletenessCheckPassed { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MinLength(1)]
        public List<CreateGoodsReceiptItemDto> Items { get; set; } = new();
    }

    public class GoodsReceiptItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int AcceptedQuantity { get; set; }
        public int RejectedQuantity { get; set; }
        public string? Note { get; set; }
    }

    public class GoodsReceiptDto : AuditableDto
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public int? PurchaseOrderId { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public WarehouseReceiptStatus Status { get; set; }
        public bool QualityCheckPassed { get; set; }
        public bool CompletenessCheckPassed { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string? Notes { get; set; }
        public IReadOnlyCollection<GoodsReceiptItemDto> Items { get; set; } = new List<GoodsReceiptItemDto>();
    }

    public class CreateSupplierReturnItemDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }

    public class CreateSupplierReturnDto
    {
        [Range(1, int.MaxValue)]
        public int SupplierId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;

        [MinLength(1)]
        public List<CreateSupplierReturnItemDto> Items { get; set; } = new();
    }

    public class SupplierReturnItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Reason { get; set; }
    }

    public class SupplierReturnDto : AuditableDto
    {
        public int Id { get; set; }
        public string ReturnNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public SupplierReturnStatus Status { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime? SentAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public IReadOnlyCollection<SupplierReturnItemDto> Items { get; set; } = new List<SupplierReturnItemDto>();
    }

    public class CreateWarehouseTransferDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        public int? FromZoneId { get; set; }

        [Range(1, int.MaxValue)]
        public int ToZoneId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }

    public class WarehouseTransferDto : AuditableDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? FromZoneId { get; set; }
        public string? FromZoneName { get; set; }
        public int ToZoneId { get; set; }
        public string? ToZoneName { get; set; }
        public int Quantity { get; set; }
        public WarehouseTransferStatus Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Note { get; set; }
    }

    public class WarehouseDocumentDto : AuditableDto
    {
        public int Id { get; set; }
        public WarehouseDocumentType Type { get; set; }
        public string Number { get; set; } = string.Empty;
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class StockForecastItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public int? MaxStockLevel { get; set; }
        public int RecommendedOrderQuantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class StockForecastDto
    {
        public DateTime GeneratedAt { get; set; }
        public IReadOnlyCollection<StockForecastItemDto> Items { get; set; } = new List<StockForecastItemDto>();
    }
}
