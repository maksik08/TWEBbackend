namespace ProjectBackend.api.Models.DTO
{
    public class ProductListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }

        public int? CategoryId { get; set; }

        public int? SupplierId { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }
    }
}
