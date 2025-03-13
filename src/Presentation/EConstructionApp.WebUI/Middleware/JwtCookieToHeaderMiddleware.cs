using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class JwtCookieToHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public JwtCookieToHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("AuthToken", out var token))
        {
            context.Request.Headers["Authorization"] = $"Bearer {token}";
        }

        await _next(context);
    }
}
