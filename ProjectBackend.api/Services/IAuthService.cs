using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken);
        Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken);
    }
}
