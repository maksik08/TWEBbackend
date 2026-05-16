using System.ComponentModel.DataAnnotations;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.DTO
{
    public class UpdateOrderStatusDto
    {
        [Required]
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; }
    }
}
