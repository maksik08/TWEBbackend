using Microsoft.Extensions.Options;
using ProjectBackend.api.Configuration;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Services
{
    public class ConsolePasswordResetNotifier : IPasswordResetNotifier
    {
        private readonly PasswordResetOptions _options;
        private readonly ILogger<ConsolePasswordResetNotifier> _logger;

        public ConsolePasswordResetNotifier(
            IOptions<PasswordResetOptions> options,
            ILogger<ConsolePasswordResetNotifier> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Task NotifyAsync(UserDomain user, string rawToken, CancellationToken cancellationToken)
        {
            var url = $"{_options.FrontendBaseUrl.TrimEnd('/')}{_options.ResetPath}/{rawToken}";
            _logger.LogInformation(
                "Password reset link for {Email} (user id {UserId}): {Url}",
                user.Email,
                user.Id,
                url);
            return Task.CompletedTask;
        }
    }
}
