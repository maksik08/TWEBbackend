using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Services
{
    public interface ICurrentUserContext
    {
        int? UserId { get; }
        UserRole? Role { get; }
    }
}
