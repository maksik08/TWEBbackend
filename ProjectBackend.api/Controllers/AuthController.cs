using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Authentication endpoints for guests.
    /// Access: guest only.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user account.
        /// Access: guest.
        /// </summary>
        [GuestOnly]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
        {
            var response = await _authService.RegisterAsync(dto, cancellationToken);
            return Ok(ApiResponse<AuthResponseDto>.Ok(response, "User registered successfully."));
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// Access: guest.
        /// </summary>
        [GuestOnly]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
        {
            var response = await _authService.LoginAsync(dto, cancellationToken);
            return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Authentication completed successfully."));
        }
    }
}
