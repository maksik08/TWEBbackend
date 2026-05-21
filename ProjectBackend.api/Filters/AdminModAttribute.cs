using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AdminModAttribute : AccessFilterBaseAttribute
    {
        public AdminModAttribute()
        {
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
            if (!HasAnyRole(role, UserRole.Admin))
            {
                context.Result = BuildResult(StatusCodes.Status403Forbidden, "Admin access is required.");
            }
        }
    }
}
