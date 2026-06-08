using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ResolveReturnDto
    {
        [StringLength(1000)]
        public string? Resolution { get; set; }
    }
}
