using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectBackend.api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class GuestOnlyAttribute : AccessFilterBaseAttribute
    {
        public GuestOnlyAttribute()
        {
            Order = 100;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (IsAuthenticated(context))
            {
                context.Result = BuildResult(StatusCodes.Status403Forbidden, "This action is available only for guests.");
            }
        }
    }
}
