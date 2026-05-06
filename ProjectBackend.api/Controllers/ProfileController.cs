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
        /// Access: authenticated user or admin.
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            if (!_currentUserContext.UserId.HasValue)
            {
                return Unauthorized(ApiResponse<object?>.Fail("Invalid user identity."));
            }

            var user = await _userService.GetByIdAsync(_currentUserContext.UserId.Value, cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(user));
        }
    }
}
