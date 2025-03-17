using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EConstructionApp.WebUI.Extensions.Exceptions
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception)
            {
                context.Response.Redirect("/Error/ServerError");
            }

            if (context.Response.StatusCode == 404)
            {
                context.Response.Redirect("/Error/ServerError");
            }
            else if (context.Response.StatusCode == 401)
            {
                context.Response.Redirect("/Error/UnauthorizedError");
            }
        }


    }
}
