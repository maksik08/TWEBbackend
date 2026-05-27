using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class CreateProductReviewDto
    {
        [Range(1, 5)]
        public byte Rating { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Comment { get; set; } = string.Empty;
    }
}
