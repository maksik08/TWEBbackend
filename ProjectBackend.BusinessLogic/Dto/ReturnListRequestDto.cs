using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ReturnListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }

        public ReturnStatus? Status { get; set; }

        public int? OrderId { get; set; }
    }
}
