using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class OwnerOrAdminAttribute : AccessFilterBaseAttribute
    {
        private readonly string _routeKey;

        public OwnerOrAdminAttribute(string routeKey = "id")
        {
            _routeKey = routeKey;
            Order = int.MinValue + 1;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsAuthenticated(context))
            {
                context.Result = BuildResult(StatusCodes.Status401Unauthorized, "Authentication is required.");
                return;
            }

            if (HasAnyRole(GetRole(context), UserRole.Admin))
            {
                return;
            }

            var userIdClaim = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var currentUserId))
            {
                context.Result = BuildResult(StatusCodes.Status401Unauthorized, "Invalid user identity.");
                return;
            }

            if (!context.ActionArguments.TryGetValue(_routeKey, out var routeValue) ||
                routeValue is not int routeId)
            {
                context.Result = BuildResult(StatusCodes.Status400BadRequest, $"Route parameter '{_routeKey}' is required.");
                return;
            }

            if (routeId != currentUserId)
            {
                context.Result = BuildResult(StatusCodes.Status403Forbidden, "You can access only your own resources.");
            }
        }
    }
}
