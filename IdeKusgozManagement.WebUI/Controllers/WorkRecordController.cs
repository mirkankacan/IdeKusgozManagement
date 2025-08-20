using System.Text.Json;
using IdeKusgozManagement.WebUI.Models.WorkRecordModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("puantaj")]
    public class WorkRecordController : Controller
    {
        [Authorize(Roles = "Admin,Şef")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin,Şef,Personel")]
        [HttpGet("ekle")]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin,Şef,Personel")]
        [HttpPost("ekle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateWorkRecordViewModel model, CancellationToken cancellationToken = default)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

         
            return Ok();

        }
        [Authorize(Roles = "Admin,Şef")]
        [HttpGet("liste/{userId}")]
        public async Task<IActionResult> ListByUser(string userId, CancellationToken cancellationToken = default)
        {
            return Ok();
        }

        [Authorize(Roles = "Admin,Şef")]
        [HttpPut("guncelle/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] List<string> workRecords, CancellationToken cancellationToken = default)
        {
            return Ok();
        }
    }

}