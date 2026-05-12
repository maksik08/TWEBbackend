using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Services
{
    public interface IPasswordResetNotifier
    {
        Task NotifyAsync(UserDomain user, string rawToken, CancellationToken cancellationToken);
    }
}
