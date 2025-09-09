using System.Security.Claims;
using IdeKusgozManagement.WebUI.Models;
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
        [HttpGet("liste")]
        public async Task<IActionResult> GetWorkRecordsByUserIdAndDate([FromQuery] string userId, [FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            var response = await _workRecordApiService.GetWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("listem")]
        public async Task<IActionResult> GetMyWorkRecords([FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            var response = await _workRecordApiService.GetMyWorkRecordsByDateAsync(date, cancellationToken);
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
        public async Task<IActionResult> CreateWorkRecord([FromBody] List<CreateWorkRecordViewModel> model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _workRecordApiService.BatchCreateWorkRecordsAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPost("toplu-reddet")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchRejectWorkRecordsByUserId([FromQuery] string userId, [FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş geçilemez");
            }
            var response = await _workRecordApiService.BatchRejectWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Şef, Yönetici")]
        [HttpPost("toplu-onayla")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchApproveWorkRecordsByUserId([FromQuery] string userId, [FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş geçilemez");
            }
            var response = await _workRecordApiService.BatchApproveWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPut("toplu-guncelle")]
        public async Task<IActionResult> BatchUpdateWorkRecordsByUserId([FromQuery] string? userId, [FromBody] List<UpdateWorkRecordViewModel> model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            if (roleClaim == null)
            {
                return BadRequest("Rol bilgisi bulunamadı");
            }

            ApiResponse<IEnumerable<WorkRecordViewModel>> response;

            switch (roleClaim.Value)
            {
                case "Admin":
                case "Yönetici":
                case "Şef":
                    if (string.IsNullOrEmpty(userId))
                    {
                        return BadRequest("Kullanıcı ID'si boş geçilemez");
                    }
                    response = await _workRecordApiService.BatchUpdateWorkRecordsByUserIdAsync(userId, model, cancellationToken);
                    break;

                case "Peronsel":
                    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(currentUserId))
                    {
                        return BadRequest("Kullanıcı kimliği bulunamadı");
                    }
                    response = await _workRecordApiService.BatchUpdateWorkRecordsByUserIdAsync(currentUserId, model, cancellationToken);
                    break;

                default:
                    return Forbid("Yetkisiz erişim");
            }

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}