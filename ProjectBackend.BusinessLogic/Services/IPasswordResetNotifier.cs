using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IPasswordResetNotifier
    {
        Task NotifyAsync(UserDomain user, string rawToken, CancellationToken cancellationToken);
    }
}
