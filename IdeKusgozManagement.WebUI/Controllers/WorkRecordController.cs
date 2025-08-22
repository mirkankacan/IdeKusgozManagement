using System.Threading.Tasks;
using IdeKusgozManagement.WebUI.Models.WorkRecordModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("puantaj")]
    public class WorkRecordController : Controller
    {
        private readonly IWorkRecordApiService _workRecordApiService;
        public WorkRecordController(IWorkRecordApiService workRecordApiService)
        {
            _workRecordApiService = workRecordApiService;
        }

        [Authorize(Roles = "Admin,Yönetici,Şef")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin,Yönetici,Şef")]
        [HttpGet("kullanici-listesi")]
        public async Task<IActionResult> GetListByDateAndUser(DateTime date, string userId, CancellationToken cancellationToken = default)
        {
            var response = await _workRecordApiService.GetWorkRecordsByDateAndUserAsync(date,userId,cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("ekle")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost("toplu-ekle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] List<CreateWorkRecordViewModel> model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _workRecordApiService.BatchCreateWorkRecordsAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpGet("toplu-reddet")]
        public async Task<IActionResult> Reject([FromQuery] string userId, [FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş geçilemez");
            }
            var response = await _workRecordApiService.BatchRejectWorkRecordsByUserAndDateAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpGet("toplu-onayla")]
        public async Task<IActionResult> Approve([FromQuery] string userId, [FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş geçilemez");
            }
            var response = await _workRecordApiService.BatchApproveWorkRecordsByUserAndDateAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("guncelle/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateWorkRecordViewModel model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Puantaj ID'si boş geçilemez");
            }
            var response = await _workRecordApiService.UpdateWorkRecordAsync(id, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}