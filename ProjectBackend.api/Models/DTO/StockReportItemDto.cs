namespace ProjectBackend.api.Models.DTO
{
    public class StockReportItemDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsVisible { get; set; }
    }
}
