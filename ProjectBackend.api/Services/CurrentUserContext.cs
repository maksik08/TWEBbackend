using System.Security.Claims;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Services
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

        public UserRole? Role
        {
            get
            {
                var claimValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
                return Enum.TryParse<UserRole>(claimValue, ignoreCase: true, out var role) ? role : null;
            }
        }
    }
}
