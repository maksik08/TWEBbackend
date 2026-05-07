using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken);

        Task<AuthResult> LoginAsync(LoginDto dto, CancellationToken cancellationToken);

        Task<AuthResult> RefreshAsync(string rawRefreshToken, CancellationToken cancellationToken);

        Task LogoutAsync(string rawRefreshToken, CancellationToken cancellationToken);
    }
}
