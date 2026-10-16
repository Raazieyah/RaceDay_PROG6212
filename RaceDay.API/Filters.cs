uusing Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Api.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public SessionAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetString("UserId");
            var userRole = session.GetString("Role");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                context.Result = new UnauthorizedObjectResult("You must be logged in.");
                return;
            }

            if (_roles.Length > 0 && !_roles.Contains(userRole))
            {
                context.Result = new ObjectResult("Access Denied: Insufficient permissions.") { StatusCode = 403 };
            }
        }
    }
}

