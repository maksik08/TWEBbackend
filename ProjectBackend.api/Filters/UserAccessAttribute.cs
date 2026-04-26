using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class UserAccessAttribute : AccessFilterBaseAttribute
    {
        public UserAccessAttribute()
        {
            Order = 0;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsAuthenticated(context))
            {
                context.Result = BuildResult(StatusCodes.Status401Unauthorized, "Authentication is required.");
                return;
            }

            var role = GetRole(context);
            if (!HasAnyRole(role, UserRole.User, UserRole.Customer, UserRole.Admin))
            {
                context.Result = BuildResult(StatusCodes.Status403Forbidden, "User access is required.");
            }
        }
    }
}
