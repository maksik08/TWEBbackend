using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Administrative operations for user management.
    /// Access: admin only for every endpoint in this controller.
    /// </summary>
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

        /// <summary>
        /// Returns a paged list of users.
        /// Access: admin.
        /// </summary>
        [AdminMod]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserListRequestDto request, CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<UserDto>.Ok(users));
        }

        /// <summary>
        /// Returns one user by identifier.
        /// Access: admin.
        /// </summary>
        [AdminMod]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(user));
        }

        /// <summary>
        /// Creates a user.
        /// Access: admin.
        /// </summary>
        [AdminMod]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
        {
            var created = await _userService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<UserDto>.Ok(created, "User created successfully."));
        }

        /// <summary>
        /// Updates a user.
        /// Access: admin.
        /// </summary>
        [AdminMod]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var updated = await _userService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(updated, "User updated successfully."));
        }

        /// <summary>
        /// Deletes a user.
        /// Access: admin.
        /// </summary>
        [AdminMod]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var deleted = await _userService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse<UserDto>.Ok(deleted, "User deleted successfully."));
        }
    }
}
