using AppZeroAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace AppZeroAPI.Shared
{
    public class AuthorizeAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.IsInRole(Role.Admin))
            {
                context.Result = AppResponse.Forbidden("Forbid Result");
                //context.Result = new ForbidResult();
            }
        }
    }

    public class AuthorizeUserAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.IsInRole(Role.User))
            {
                var xxx = JsonConvert.SerializeObject(new { mssg = "xxxxx" }, Formatting.Indented);
                //context.Result = AppResponse.Forbidden("Forbid Result");
                context.Result = new ForbidResult(xxx);
            }
        }
    }

    public class AuthorizeClientAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.IsInRole(Role.Client))
            {
                context.Result = AppResponse.Forbidden("Forbid Result");
                // context.Result = new ForbidResult();
            }
        }
    }


}