using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Filters
{
    public abstract class AccessFilterBaseAttribute : ActionFilterAttribute
    {
        protected static string? GetRole(ActionExecutingContext context)
        {
            return context.HttpContext.User.FindFirstValue(ClaimTypes.Role);
        }

        protected static bool IsAuthenticated(ActionExecutingContext context)
        {
            return context.HttpContext.User.Identity?.IsAuthenticated == true;
        }

        protected static ObjectResult BuildResult(int statusCode, string message)
        {
            return new ObjectResult(new { message }) { StatusCode = statusCode };
        }

        protected static bool HasAnyRole(string? role, params UserRole[] roles)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            return roles.Any(expected =>
                string.Equals(role, expected.ToString(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
