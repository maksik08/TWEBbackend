namespace ProjectBackend.BusinessLogic.Dto
{
    public class StockReportDto
    {
        public int TotalProducts { get; set; }

        public int VisibleProducts { get; set; }

        public int TotalUnitsInStock { get; set; }

        public ICollection<StockReportItemDto> Items { get; set; } = new List<StockReportItemDto>();
    }
}
