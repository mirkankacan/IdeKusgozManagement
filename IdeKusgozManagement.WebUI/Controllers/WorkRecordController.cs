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
        public async Task<IActionResult> GetListByDateAndUser([FromQuery] DateTime date, [FromQuery] string userId, CancellationToken cancellationToken = default)
        {
            var response = await _workRecordApiService.GetWorkRecordsByDateAndUserAsync(date, userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("listem")]
        public async Task<IActionResult> GetMyList([FromQuery] DateTime date, CancellationToken cancellationToken = default)
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
        [HttpPost("toplu-reddet")]
        [ValidateAntiForgeryToken]
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
        [HttpPost("toplu-onayla")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve([FromQuery] string userId, [FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si boş geçilemez");
            }
            var response = await _workRecordApiService.BatchApproveWorkRecordsByUserAndDateAsync(userId, date, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPut("guncelle")]
        public async Task<IActionResult> Update([FromQuery] string userId, [FromBody] List<UpdateWorkRecordViewModel> model, CancellationToken cancellationToken = default)
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
                    response = await _workRecordApiService.BatchUpdateWorkRecordByUserAsync(userId, model, cancellationToken);
                    break;

                case "Peronsel":
                    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(currentUserId))
                    {
                        return BadRequest("Kullanıcı kimliği bulunamadı");
                    }
                    response = await _workRecordApiService.BatchUpdateWorkRecordByUserAsync(currentUserId, model, cancellationToken);
                    break;

                default:
                    return Forbid("Yetkisiz erişim");
            }

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}