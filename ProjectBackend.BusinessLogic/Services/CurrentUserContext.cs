using System.Security.Claims;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Services
{
    public class CurrentUserContext : ICurrentUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var claimValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(claimValue, out var userId) ? userId : null;
            }
        }

        public string? Username => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

        public UserRole? Role
        {
            get
            {
                var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
                return Enum.TryParse<UserRole>(roleClaim, ignoreCase: true, out var role)
                    ? role
                    : null;
            }
        }
    }
}
