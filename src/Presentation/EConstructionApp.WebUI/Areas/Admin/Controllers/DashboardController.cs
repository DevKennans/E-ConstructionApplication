using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
