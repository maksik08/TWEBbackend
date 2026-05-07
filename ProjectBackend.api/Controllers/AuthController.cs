using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Authentication endpoints.
    /// Login/Register: guest only.
    /// Refresh/Logout: any caller — authorization is performed via the refresh-token cookie.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string RefreshCookieName = "refresh_token";
        private const string RefreshCookiePath = "/api/auth";

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        [GuestOnly]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(dto, cancellationToken);
            SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
            return Ok(ApiResponse<AuthResponseDto>.Ok(result.Response, "User registered successfully."));
        }

        /// <summary>
        /// Authenticates a user, returns a JWT access token in the body and sets a httpOnly refresh cookie.
        /// </summary>
        [GuestOnly]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(dto, cancellationToken);
            SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
            return Ok(ApiResponse<AuthResponseDto>.Ok(result.Response, "Authentication completed successfully."));
        }

        /// <summary>
        /// Issues a new access token using the refresh-token cookie. Rotates the cookie.
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            var rawRefresh = Request.Cookies[RefreshCookieName];
            var result = await _authService.RefreshAsync(rawRefresh ?? string.Empty, cancellationToken);
            SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
            return Ok(ApiResponse<AuthResponseDto>.Ok(result.Response, "Token refreshed."));
        }

        /// <summary>
        /// Revokes the refresh token tied to the current cookie and clears it.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var rawRefresh = Request.Cookies[RefreshCookieName];
            if (!string.IsNullOrWhiteSpace(rawRefresh))
            {
                await _authService.LogoutAsync(rawRefresh, cancellationToken);
            }

            ClearRefreshCookie();
            return Ok(ApiResponse<object?>.Ok(null, "Logged out."));
        }

        private void SetRefreshCookie(string token, DateTime expiresAt)
        {
            Response.Cookies.Append(RefreshCookieName, token, BuildCookieOptions(expiresAt));
        }

        private void ClearRefreshCookie()
        {
            Response.Cookies.Append(RefreshCookieName, string.Empty, BuildCookieOptions(DateTime.UnixEpoch));
        }

        private static CookieOptions BuildCookieOptions(DateTime expiresAt)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = RefreshCookiePath,
                Expires = expiresAt
            };
        }
    }
}
