using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Services
{
    public interface ICurrentUserContext
    {
        int? UserId { get; }
        string? Username { get; }
        UserRole? Role { get; }
    }
}
