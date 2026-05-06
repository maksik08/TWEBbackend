using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.DTO
{
    public class UserListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }

        public UserRole? Role { get; set; }
    }
}
