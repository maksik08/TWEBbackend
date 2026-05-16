using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RoleAccessAttribute : AccessFilterBaseAttribute
    {
        private readonly UserRole[] _roles;

        public RoleAccessAttribute(params UserRole[] roles)
        {
            _roles = roles;
            Order = int.MinValue;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsAuthenticated(context))
            {
                context.Result = BuildResult(StatusCodes.Status401Unauthorized, "Authentication is required.");
                return;
            }

            var role = GetRole(context);
            if (!HasAnyRole(role, _roles) && !HasAnyRole(role, UserRole.Admin))
            {
                context.Result = BuildResult(StatusCodes.Status403Forbidden, "You do not have permission to access this resource.");
            }
        }
    }
}
