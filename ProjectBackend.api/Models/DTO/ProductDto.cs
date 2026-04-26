namespace ProjectBackend.api.Models.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int? CategoryId { get; set; }
        public string? Category { get; set; }
        public int? SupplierId { get; set; }
        public string? Supplier { get; set; }
    }
}
