using AppZeroAPI.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AppZeroAPI.Middleware
{
    public class ExpiredTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public ExpiredTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Response.Headers["Token-Expired"] == "true")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                var resp = AppResponse.UnauthorizedUser("Authorization Rejection - Token-Expired");
                await context.Response.WriteAsync(JsonConvert.SerializeObject(resp, Formatting.Indented));


                // DO NOT CALL NEXT. THIS SHORTCIRCUITS THE PIPELINE
            }
            else if (context.Response.Headers["Token-Invalid"] == "true")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                var resp = AppResponse.UnauthorizedUser("Authorization Rejection - Token-Invalid");
                await context.Response.WriteAsync(JsonConvert.SerializeObject(resp, Formatting.Indented));
            }
            else if (context.Response.Headers["Token-Invalid-Signature"] == "true")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                var resp = AppResponse.UnauthorizedUser("Authorization Rejection - Token-Invalid-Signature");
                await context.Response.WriteAsync(JsonConvert.SerializeObject(resp, Formatting.Indented));
            }
            else
            {
                await _next(context);
            }
        }
    }
}
