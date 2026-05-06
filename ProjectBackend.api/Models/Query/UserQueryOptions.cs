using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Models.Query
{
    public sealed class UserQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }

        public UserRole? Role { get; init; }
    }
}
