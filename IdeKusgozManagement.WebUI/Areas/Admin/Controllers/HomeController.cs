using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("admin")]
    public class HomeController : Controller
    {
        [HttpGet("ana-sayfa")]
        public IActionResult Index()
        {
            return View();
        }
    }
}