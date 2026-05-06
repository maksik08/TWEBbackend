using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Operations for the current authenticated profile.
    /// Access: authenticated user or admin.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [UserAccess]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserContext _currentUserContext;

        public ProfileController(IUserService userService, ICurrentUserContext currentUserContext)
        {
            _userService = userService;
            _currentUserContext = currentUserContext;
        }

        /// <summary>
        /// Returns the currently authenticated user profile.
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(RequireUserId(), cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(user));
        }

        /// <summary>
        /// Updates first/last name and phone for the current user.
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken cancellationToken)
        {
            var user = await _userService.UpdateProfileAsync(RequireUserId(), dto, cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(user, "Profile updated successfully."));
        }

        /// <summary>
        /// Tops up the balance of the current user (mock payment).
        /// </summary>
        [HttpPost("me/topup")]
        public async Task<IActionResult> TopUp([FromBody] TopUpBalanceDto dto, CancellationToken cancellationToken)
        {
            var user = await _userService.TopUpBalanceAsync(RequireUserId(), dto.Amount, cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(user, "Balance topped up successfully."));
        }

        private int RequireUserId()
        {
            if (!_currentUserContext.UserId.HasValue)
            {
                throw new Exceptions.UnauthorizedAppException("Invalid user identity.");
            }

            return _currentUserContext.UserId.Value;
        }
    }
}
