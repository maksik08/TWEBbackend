using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [GuestOnly]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (response, error) = await _authService.RegisterAsync(dto);
            if (error is not null) return Conflict(new { message = error });
            return Ok(response);
        }

        [GuestOnly]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (response, error) = await _authService.LoginAsync(dto);
            if (error is not null) return Unauthorized(new { message = error });
            return Ok(response);
        }
    }
}
