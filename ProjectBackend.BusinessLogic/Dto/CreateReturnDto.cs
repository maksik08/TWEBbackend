using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class CreateReturnDto
    {
        [Range(1, int.MaxValue)]
        public int OrderId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 5)]
        public string Reason { get; set; } = string.Empty;
    }
}
