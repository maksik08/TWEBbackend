using ProjectBackend.Domain.Entities;

namespace ProjectBackend.Domain.Query
{
    public sealed class UserQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }

        public UserRole? Role { get; init; }
    }
}
