using IdeKusgozManagement.WebUI.Models.WorkRecordModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("puantaj")]
    public class WorkRecordController : Controller
    {
        private readonly IWorkRecordApiService _workRecordApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WorkRecordController(IWorkRecordApiService workRecordApiService, IHttpContextAccessor httpContextAccessor)
        {
            _workRecordApiService = workRecordApiService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin,Yönetici,Şef")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Yönetici,Şef")]
        [HttpGet("liste/kullanici/{userId}/tarih/{date:datetime}")]
        public async Task<IActionResult> GetWorkRecordsByUserIdAndDate(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            var response = await _workRecordApiService.GetWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("listem/tarih/{date:datetime}")]
        public async Task<IActionResult> GetMyWorkRecords(DateTime date, CancellationToken cancellationToken = default)
        {
            var response = await _workRecordApiService.GetMyWorkRecordsByDateAsync(date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("ekle")]
        public IActionResult Create()
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Lütfen tekrar giriş yapınız");

            ViewData["UserId"] = userId;
            return View();
        }

        [Authorize]
        [HttpPost("toplu-ekle-guncelle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrModifyWorkRecord([FromForm] List<CreateOrModifyWorkRecordViewModel> model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _workRecordApiService.BatchCreateOrModifyWorkRecordsAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("toplu-reddet/kullanici/{userId}/tarih/{date}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchRejectWorkRecordsByUserId(string userId, DateTime date, [FromQuery] string? rejectReason, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş geçilemez");
            }
            var response = await _workRecordApiService.BatchRejectWorkRecordsByUserIdAndDateAsync(userId, date, rejectReason, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("toplu-onayla/kullanici/{userId}/tarih/{date}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchApproveWorkRecordsByUserId(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş geçilemez");
            }
            var response = await _workRecordApiService.BatchApproveWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("toplu-guncelle/kullanici/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchUpdateWorkRecordsByUserId(string userId, [FromForm] List<CreateOrModifyWorkRecordViewModel> model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _workRecordApiService.BatchUpdateWorkRecordsByUserIdAsync(userId, model, cancellationToken);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("onayla/{workRecordId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveById(string workRecordId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(workRecordId))
            {
                return BadRequest("Puantaj ID'si boş geçilemez");
            }
            var response = await _workRecordApiService.ApproveWorkRecordByIdAsync(workRecordId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("reddet/{workRecordId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectById(string workRecordId, [FromQuery] string? rejectReason, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(workRecordId))
            {
                return BadRequest("Puantaj ID'si boş geçilemez");
            }
            var response = await _workRecordApiService.RejectWorkRecordByIdAsync(workRecordId, rejectReason, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}