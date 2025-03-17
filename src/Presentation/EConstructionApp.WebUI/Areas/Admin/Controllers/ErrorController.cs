using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ErrorController : Controller
    {
        public IActionResult ServerError()
        {
            return View();
        }

        public IActionResult UnauthorizedError()
        {
            return View();
        }
    }
}
