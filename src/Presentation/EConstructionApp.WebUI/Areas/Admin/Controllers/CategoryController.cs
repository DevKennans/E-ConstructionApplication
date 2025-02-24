using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        public IActionResult AddCategory()
        {
            return View();
        }
    }
}
