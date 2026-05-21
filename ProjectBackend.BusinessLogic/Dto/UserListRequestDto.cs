using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class UserListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }

        public UserRole? Role { get; set; }
    }
}
