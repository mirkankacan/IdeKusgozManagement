using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{

    [Route("")]
    public class HomeController : Controller
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/ana-sayfa")]
        public IActionResult AdminIndex()
        {

            return View();
        }
        [Authorize(Roles = "Şef")]
        [HttpGet("sef/ana-sayfa")]
        public IActionResult SefIndex()
        {

            return View();
        }
        [Authorize(Roles = "Personel")]
        [HttpGet("personel/ana-sayfa")]
        public IActionResult PersonelIndex()
        {

            return View();
        }
        [Authorize]
        [HttpGet("takvim")]
        public IActionResult Calendar()
        {
            return View();
        }
    }
}