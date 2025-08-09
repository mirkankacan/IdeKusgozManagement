using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Areas.Personel.Controllers
{
    [Area("Personel")]
    [Authorize(Roles = "Personel")]
    [Route("personel")]
    public class HomeController : Controller
    {
        [HttpGet("ana-sayfa")]
        public IActionResult Index()
        {
            return View();
        }
    }
}