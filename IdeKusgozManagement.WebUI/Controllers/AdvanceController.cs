using IdeKusgozManagement.WebUI.Extensions;
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

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetAdvances(CancellationToken cancellationToken)
        {
            var response = await _advanceApiService.GetAdvancesAsync(cancellationToken);

            return response.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("istatistik")]
        public async Task<IActionResult> GetAdvanceStatistics(CancellationToken cancellationToken)
        {
            var response = await _advanceApiService.GetAdvanceStatisticsAsync(cancellationToken);

            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("listem")]
        public async Task<IActionResult> GetMyAdvances(CancellationToken cancellationToken)
        {
            var response = await _advanceApiService.GetMyAdvancesAsync(cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("onaylanmis")]
        public IActionResult Approved()
        {
            return View();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("onaylanmis-liste")]
        public async Task<IActionResult> GetApprovedAdvances(CancellationToken cancellationToken)
        {
            var response = await _advanceApiService.GetApprovedAdvancesAsync(cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("tamamlanmis-liste")]
        public async Task<IActionResult> GetCompletedAdvances(CancellationToken cancellationToken)
        {
            var response = await _advanceApiService.GetCompletedAdvancesAsync(cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("kullanici/{userId}")]
        public async Task<IActionResult> GetAdvancesByUser(string userId, CancellationToken cancellationToken)
        {
            var response = await _advanceApiService.GetAdvancesByUserIdAsync(userId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("{advanceId}")]
        public async Task<IActionResult> GetAdvanceById(string advanceId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var response = await _advanceApiService.GetAdvanceByIdAsync(advanceId, cancellationToken);
            return response.ToActionResult();
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
        public async Task<IActionResult> Create([FromBody] CreateAdvanceViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _advanceApiService.CreateAdvanceAsync(model, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPut("{advanceId}")]
        public async Task<IActionResult> UpdateAdvance(string advanceId, [FromBody] UpdateAdvanceViewModel model, CancellationToken cancellationToken)
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
            return response.ToActionResult();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpDelete("{advanceId}")]
        public async Task<IActionResult> DeleteAdvance(string advanceId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var response = await _advanceApiService.DeleteAdvanceAsync(advanceId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [ValidateAntiForgeryToken]
        [HttpPut("{advanceId}/tamamla")]
        public async Task<IActionResult> CompleteAdvance(string advanceId, [FromBody] ApproveAdvanceViewModel? model = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var response = await _advanceApiService.CompleteAdvanceAsync(advanceId, model, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [ValidateAntiForgeryToken]
        [HttpPut("{advanceId}/onayla")]
        public async Task<IActionResult> ApproveAdvance(string advanceId, [FromBody] ApproveAdvanceViewModel? model = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var response = await _advanceApiService.ApproveAdvanceAsync(advanceId, model, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [ValidateAntiForgeryToken]
        [HttpPut("{advanceId}/reddet")]
        public async Task<IActionResult> RejectAdvance(string advanceId, [FromQuery] string? rejectReason, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var response = await _advanceApiService.RejectAdvanceAsync(advanceId, rejectReason, cancellationToken);
            return response.ToActionResult();
        }
    }
}