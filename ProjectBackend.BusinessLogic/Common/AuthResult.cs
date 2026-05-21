using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Common
{
    public class AuthResult
    {
        public required AuthResponseDto Response { get; init; }

        public required string RefreshToken { get; init; }

        public required DateTime RefreshTokenExpiresAt { get; init; }
    }
}
