using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.AdvanceModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("avans")]
    public class AdvanceController : Controller
    {
        private readonly IAdvanceApiService _advanceApiService;

        public AdvanceController(IAdvanceApiService advanceApiService)
        {
            _advanceApiService = advanceApiService;
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetAdvances(CancellationToken cancellationToken = default)
        {

            ApiResponse<IEnumerable<AdvanceViewModel>> response;
            if (User.IsInRole("Admin"))
            {
                response = await _advanceApiService.GetAdvancesAsync(cancellationToken);
            }
            else if (User.IsInRole("Yönetici"))
            {
                response = await _advanceApiService.GetChiefProcessedAdvancesAsync(cancellationToken);
            }
            else if (User.IsInRole("Şef"))
            {
                response = await _advanceApiService.GetAdvancesAsync(cancellationToken);
            }
            else
            {
                return Forbid();
            }
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("listem")]
        public async Task<IActionResult> GetMyAdvances(CancellationToken cancellationToken = default)
        {
            var response = await _advanceApiService.GetMyAdvancesAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("kullanici/{userId}")]
        public async Task<IActionResult> GetAdvancesByUser(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _advanceApiService.GetAdvancesByUserIdAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("{advanceId}")]
        public async Task<IActionResult> GetAdvanceById(string advanceId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var response = await _advanceApiService.GetAdvanceByIdAsync(advanceId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("istek-olustur")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("istek-olustur")]
        public async Task<IActionResult> Create([FromBody] CreateAdvanceViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _advanceApiService.CreateAdvanceAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPut("{advanceId}")]
        public async Task<IActionResult> UpdateAdvance(string advanceId, [FromBody] UpdateAdvanceViewModel model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _advanceApiService.UpdateAdvanceAsync(advanceId, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpDelete("{advanceId}")]
        public async Task<IActionResult> DeleteAdvance(string advanceId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var response = await _advanceApiService.DeleteAdvanceAsync(advanceId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{advanceId}/onayla")]
        public async Task<IActionResult> ApproveAdvance(string advanceId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var response = await _advanceApiService.ApproveAdvanceAsync(advanceId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{advanceId}/reddet")]
        public async Task<IActionResult> RejectAdvance(string advanceId, [FromQuery] string? rejectReason, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var response = await _advanceApiService.RejectAdvanceAsync(advanceId, rejectReason, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}