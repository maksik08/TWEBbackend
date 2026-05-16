using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public bool IsPreorder { get; set; }

        public bool IsVisible { get; set; } = true;

        public int? CategoryId { get; set; }

        public int? SupplierId { get; set; }
    }
}
