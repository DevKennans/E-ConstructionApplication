using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MaterialController : Controller
    {
        public IActionResult AddMaterial()
        {
            return View();
        }
    }
}
