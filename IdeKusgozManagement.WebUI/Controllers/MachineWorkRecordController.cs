using IdeKusgozManagement.WebUI.Authorization;
using IdeKusgozManagement.WebUI.Models.MachineWorkRecordModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("makine-puantaj")]
    public class MachineWorkRecordController : Controller
    {
        private readonly IMachineWorkRecordApiService _MachineWorkRecordApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MachineWorkRecordController(IMachineWorkRecordApiService MachineWorkRecordApiService, IHttpContextAccessor httpContextAccessor)
        {
            _MachineWorkRecordApiService = MachineWorkRecordApiService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin,Yönetici,Şef")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Yönetici")]
        [HttpGet("liste/onaylanmis/kullanici/{userId}/tarih/{date:datetime}")]
        public async Task<IActionResult> GetApprovedMachineWorkRecordsByUser(string userId, DateTime date, int status, CancellationToken cancellationToken)
        {
            var response = await _MachineWorkRecordApiService.GetApprovedMachineWorkRecordsByUserAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin,Yönetici")]
        [HttpGet("liste/kullanici/{userId}/tarih/{date:datetime}/durum/{status:int}")]
        public async Task<IActionResult> GetMachineWorkRecordsByUserIdAndDate(string userId, DateTime date, int status, CancellationToken cancellationToken)
        {
            var response = await _MachineWorkRecordApiService.GetMachineWorkRecordsByUserIdDateStatusAsync(userId, date, status, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin,Yönetici,Şef")]
        [HttpGet("liste/kullanici/{userId}/tarih/{date:datetime}")]
        public async Task<IActionResult> GetMachineWorkRecordsByUserIdAndDate(string userId, DateTime date, CancellationToken cancellationToken)
        {
            var response = await _MachineWorkRecordApiService.GetMachineWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("listem/tarih/{date:datetime}")]
        public async Task<IActionResult> GetMyMachineWorkRecords(DateTime date, CancellationToken cancellationToken)
        {
            var response = await _MachineWorkRecordApiService.GetMyMachineWorkRecordsByDateAsync(date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [DepartmentDuty("Şoför-Yük Taşıma", "Vinç Operatörü", "Platform Operatörü")]
        [HttpGet("ekle")]
        public IActionResult Create()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return Unauthorized("Lütfen tekrar giriş yapınız");

            var userId = httpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Lütfen tekrar giriş yapınız");

            ViewData["UserId"] = userId;
            return View();
        }

        [Authorize]
        [HttpPost("toplu-ekle-duzenle")]
        [DepartmentDuty("Şoför-Yük Taşıma", "Vinç Operatörü", "Platform Operatörü")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrModifyMachineWorkRecord([FromForm] List<CreateOrModifyMachineWorkRecordViewModel> model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _MachineWorkRecordApiService.BatchCreateOrModifyMachineWorkRecordsAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("toplu-reddet/kullanici/{userId}/tarih/{date}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchRejectMachineWorkRecordsByUserId(string userId, DateTime date, [FromQuery] string? rejectReason, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş geçilemez");
            }
            var response = await _MachineWorkRecordApiService.BatchRejectMachineWorkRecordsByUserIdAndDateAsync(userId, date, rejectReason, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("toplu-onayla/kullanici/{userId}/tarih/{date}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchApproveMachineWorkRecordsByUserId(string userId, DateTime date, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş geçilemez");
            }
            var response = await _MachineWorkRecordApiService.BatchApproveMachineWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("toplu-guncelle/kullanici/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchUpdateMachineWorkRecordsByUserId(string userId, [FromForm] List<CreateOrModifyMachineWorkRecordViewModel> model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _MachineWorkRecordApiService.BatchUpdateMachineWorkRecordsByUserIdAsync(userId, model, cancellationToken);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("onayla/{MachineWorkRecordId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveById(string MachineWorkRecordId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(MachineWorkRecordId))
            {
                return BadRequest("Puantaj ID'si boş geçilemez");
            }
            var response = await _MachineWorkRecordApiService.ApproveMachineWorkRecordByIdAsync(MachineWorkRecordId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPut("reddet/{MachineWorkRecordId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectById(string MachineWorkRecordId, [FromQuery] string? rejectReason, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(MachineWorkRecordId))
            {
                return BadRequest("Puantaj ID'si boş geçilemez");
            }
            var response = await _MachineWorkRecordApiService.RejectMachineWorkRecordByIdAsync(MachineWorkRecordId, rejectReason, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}