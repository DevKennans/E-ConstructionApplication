using EConstructionApp.Application.DTOs.Identification;
using EConstructionApp.Application.Interfaces.Services.Identification;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace EConstructionApp.WebUI.Middleware
{
    public class JwtRefreshMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtRefreshMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var authToken = context.Request.Cookies["AuthToken"];
                var refreshToken = context.Request.Cookies["RefreshToken"];
                var tokenExpirationString = context.Request.Cookies["TokenExpiration"];

                if (context.Request.Path.StartsWithSegments("/Admin/Auth/Login") && !string.IsNullOrEmpty(authToken) && IsValidToken(authToken, context))
                {
                    context.Response.Redirect("/Admin/Dashboard"); 
                    return;
                }

                if (context.Request.Path.StartsWithSegments("/Admin/Auth/Login") ||
                    context.Request.Path.StartsWithSegments("/Admin/Auth/RefreshToken"))
                {
                    await _next(context);
                    return;
                }

                if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(tokenExpirationString))
                {
                    RedirectToLogin(context);
                    return;
                }

                if (string.IsNullOrEmpty(authToken) || (DateTime.TryParse(tokenExpirationString, out DateTime tokenExpiration) && DateTime.UtcNow >= tokenExpiration))
                {
                    var authService = context.RequestServices.GetRequiredService<IAuthService>();
                    var newToken = await authService.RefreshTokenAsync(refreshToken);

                    if (newToken == null)
                    {
                        ClearAuthCookies(context.Response);
                        RedirectToLogin(context);
                        return;
                    }

                    SetAuthCookies(context.Response, newToken);
                    authToken = newToken.AccessToken;
                }

                if (!IsValidToken(authToken, context))
                {
                    ClearAuthCookies(context.Response);
                    RedirectToLogin(context);
                    return;
                }

                await _next(context);
            }
            catch (Exception)
            {
                RedirectToLogin(context);
            }
        }


        private bool IsValidToken(string token, HttpContext context)
        {
            try
            {
                var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
                var securityKey = Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]);
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(securityKey),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, 
                    ClockSkew = TimeSpan.Zero, 
                    ValidIssuer = configuration["Token:Issuer"],
                    ValidAudience = configuration["Token:Audience"]
                };


                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void RedirectToLogin(HttpContext context)
        {
            context.Response.Redirect("/Admin/Auth/Login");
        }

        private void ClearAuthCookies(HttpResponse response)
        {
            response.Cookies.Delete("AuthToken");
            response.Cookies.Delete("RefreshToken");
            response.Cookies.Delete("TokenExpiration");
        }

        private void SetAuthCookies(HttpResponse response, Token token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = token.ExpirationDate
            };
            response.Cookies.Append("AuthToken", token.AccessToken, cookieOptions);
            response.Cookies.Append("RefreshToken", token.RefreshToken, cookieOptions);
            response.Cookies.Append("TokenExpiration", token.ExpirationDate.ToString("o"), cookieOptions);
        }
    }
}
