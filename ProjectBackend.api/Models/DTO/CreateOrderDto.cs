using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class CreateOrderDto
    {
        [Required]
        [MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
