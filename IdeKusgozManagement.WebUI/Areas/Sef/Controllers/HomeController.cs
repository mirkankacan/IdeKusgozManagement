using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Areas.Sef.Controllers
{
    [Area("Sef")]
    [Authorize(Roles = "Şef")]
    [Route("sef")]
    public class HomeController : Controller
    {
        [HttpGet("ana-sayfa")]
        public IActionResult Index()
        {
            return View();
        }
    }
}