using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(UserDomain user);

        DateTime GetAccessTokenExpiration();

        (string RawToken, string TokenHash, DateTime ExpiresAt) CreateRefreshToken();

        string HashRefreshToken(string rawToken);

        (string RawToken, string TokenHash, DateTime ExpiresAt) CreatePasswordResetToken();

        string HashPasswordResetToken(string rawToken);
    }
}
