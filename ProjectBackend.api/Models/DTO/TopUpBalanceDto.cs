using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.api.Models.DTO
{
    public class TopUpBalanceDto
    {
        [Range(0.01, 1_000_000)]
        public decimal Amount { get; set; }
    }
}
