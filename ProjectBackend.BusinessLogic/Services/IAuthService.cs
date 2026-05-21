using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken);

        Task<AuthResult> LoginAsync(LoginDto dto, CancellationToken cancellationToken);

        Task<AuthResult> RefreshAsync(string rawRefreshToken, CancellationToken cancellationToken);

        Task LogoutAsync(string rawRefreshToken, CancellationToken cancellationToken);

        Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken);

        Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken);
    }
}
