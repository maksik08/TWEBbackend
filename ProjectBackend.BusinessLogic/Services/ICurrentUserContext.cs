using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface ICurrentUserContext
    {
        int? UserId { get; }
        string? Username { get; }
        UserRole? Role { get; }
    }
}
