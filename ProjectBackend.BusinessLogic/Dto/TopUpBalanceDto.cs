using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class TopUpBalanceDto
    {
        [Range(0.01, 1_000_000)]
        public decimal Amount { get; set; }
    }
}
