using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.Tests.Filters
{
    public class AccessFilterTests
    {
        [Fact]
        public void AdminMod_ShouldBlockNonAdminUser()
        {
            var context = CreateContext(isAuthenticated: true, role: UserRole.User.ToString());
            var attribute = new AdminModAttribute();

            attribute.OnActionExecuting(context);

            var result = Assert.IsType<ObjectResult>(context.Result);
            Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        }

        [Fact]
        public void GuestOnly_ShouldBlockAuthenticatedUser()
        {
            var context = CreateContext(isAuthenticated: true, role: UserRole.User.ToString());
            var attribute = new GuestOnlyAttribute();

            attribute.OnActionExecuting(context);

            var result = Assert.IsType<ObjectResult>(context.Result);
            Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        }

        [Fact]
        public void UserAccess_ShouldAllowAuthenticatedUser()
        {
            var context = CreateContext(isAuthenticated: true, role: UserRole.User.ToString());
            var attribute = new UserAccessAttribute();

            attribute.OnActionExecuting(context);

            Assert.Null(context.Result);
        }

        private static ActionExecutingContext CreateContext(bool isAuthenticated, string? role = null)
        {
            var httpContext = new DefaultHttpContext();

            if (isAuthenticated)
            {
                var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, "1") };
                if (!string.IsNullOrWhiteSpace(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                httpContext.User = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, authenticationType: "TestAuth"));
            }
            else
            {
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            }

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor());

            return new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                controller: new object());
        }
    }
}
