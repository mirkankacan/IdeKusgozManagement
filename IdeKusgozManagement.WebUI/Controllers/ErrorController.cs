using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("erisim-engellendi")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}