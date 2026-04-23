using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IAuthService
    {
        Task<(AuthResponseDto? Response, string? Error)> RegisterAsync(RegisterDto dto);
        Task<(AuthResponseDto? Response, string? Error)> LoginAsync(LoginDto dto);
    }
}
