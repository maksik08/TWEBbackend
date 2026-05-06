using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UserAccess]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<object?>.Fail("Invalid user identity."));
            }

            var user = await _userService.GetByIdAsync(userId, cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(user));
        }

        [AdminMod]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserListRequestDto request, CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<UserDto>.Ok(users));
        }

        [OwnerOrAdmin]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(user));
        }

        [AdminMod]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
        {
            var created = await _userService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<UserDto>.Ok(created, "User created successfully."));
        }

        [AdminMod]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var updated = await _userService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(updated, "User updated successfully."));
        }

        [OwnerOrAdmin]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var deleted = await _userService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(deleted, "User deleted successfully."));
        }
    }
}
