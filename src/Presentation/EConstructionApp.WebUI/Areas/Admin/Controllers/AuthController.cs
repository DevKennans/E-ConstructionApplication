using EConstructionApp.Application.DTOs.Identification;
using EConstructionApp.Application.Features.Commands.Auth.LogIn;
using EConstructionApp.Application.Interfaces.Services.Identification;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loginRequest = new LogInCommandRequest
            {
                UsernameOrPhoneNumber = model.Username,
                Password = model.Password
            };

            var result = await _authService.LogInAsync(loginRequest);

            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessageFromLogin = result.Message;
                return View(model);
            }

            SetAuthCookies(result.Token);
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        public IActionResult Logout()
        {
            ClearAuthCookies();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
                return Unauthorized("Session expired. Please log in again.");

            var newToken = await _authService.RefreshTokenAsync(refreshToken);

            if (newToken == null)
            {
                ClearAuthCookies();
                return Unauthorized("Session expired. Please log in again.");
            }

            SetAuthCookies(newToken);
            return Ok(new { message = "Token refreshed successfully." });
        }

        private void SetAuthCookies(Token token)
        {
            void AppendCookie(string name, string value) =>
                Response.Cookies.Append(name, value, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = token.ExpirationDate
                });

            AppendCookie("AuthToken", token.AccessToken);
            AppendCookie("RefreshToken", token.RefreshToken);
            AppendCookie("TokenExpiration", token.ExpirationDate.ToString("o"));
        }

        private void ClearAuthCookies()
        {
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("RefreshToken");
            Response.Cookies.Delete("TokenExpiration");
        }
    }
}
