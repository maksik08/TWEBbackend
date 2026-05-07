using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Models.Common
{
    public class AuthResult
    {
        public required AuthResponseDto Response { get; init; }

        public required string RefreshToken { get; init; }

        public required DateTime RefreshTokenExpiresAt { get; init; }
    }
}
