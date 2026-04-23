using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IAuthService
    {
        Task<(UserDto? User, string? Error)> RegisterAsync(RegisterDto dto);
    }
}
