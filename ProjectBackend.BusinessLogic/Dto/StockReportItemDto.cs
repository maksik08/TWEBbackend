namespace ProjectBackend.BusinessLogic.Dto
{
    public class StockReportItemDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int ReservedQuantity { get; set; }

        public int AvailableQuantity { get; set; }

        public int MinStockLevel { get; set; }

        public int? MaxStockLevel { get; set; }

        public bool IsLowStock { get; set; }

        public bool IsVisible { get; set; }
    }
}
