using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ContactMessageListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }

        public ContactMessageStatus? Status { get; set; }
    }
}
