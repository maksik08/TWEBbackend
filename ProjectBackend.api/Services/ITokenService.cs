using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(UserDomain user);

        DateTime GetAccessTokenExpiration();

        (string RawToken, string TokenHash, DateTime ExpiresAt) CreateRefreshToken();

        string HashRefreshToken(string rawToken);
    }
}
