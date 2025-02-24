using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    public class ErrorController : Controller
    {
        [Area("Admin")]
        public IActionResult ServerError()
        {
            return View();
        }
    }
}
