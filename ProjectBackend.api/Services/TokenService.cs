using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectBackend.api.Configuration;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Services
{
    public class TokenService : ITokenService
    {
        private const int RefreshTokenByteLength = 64;

        private readonly JwtOptions _jwtOptions;

        public TokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string CreateAccessToken(UserDomain user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: GetAccessTokenExpiration(),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetAccessTokenExpiration()
        {
            return DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiryMinutes);
        }

        public (string RawToken, string TokenHash, DateTime ExpiresAt) CreateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(RefreshTokenByteLength);
            var rawToken = Base64UrlEncode(bytes);
            var hash = HashRefreshToken(rawToken);
            var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays);
            return (rawToken, hash, expiresAt);
        }

        public string HashRefreshToken(string rawToken)
        {
            var bytes = Encoding.UTF8.GetBytes(rawToken);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
