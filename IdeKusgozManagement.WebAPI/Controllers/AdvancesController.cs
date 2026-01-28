using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.AdvanceDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdvancesController(IAdvanceService advanceService, IIdentityService identityService) : ControllerBase
    {
        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetAdvanceStatistics(CancellationToken cancellationToken)
        {
            var result = await advanceService.GetAdvanceStatisticsAsync(cancellationToken);
            return result.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAdvances(CancellationToken cancellationToken)
        {
            var result = await advanceService.GetAdvancesAsync(cancellationToken);
            return result.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedAdvances(CancellationToken cancellationToken)
        {
            var result = await advanceService.GetApprovedAdvancesAsync(cancellationToken);
            return result.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet(template: "completed")]
        public async Task<IActionResult> GetCompletedAdvances(CancellationToken cancellationToken)
        {
            var result = await advanceService.GetCompletedAdvancesAsync(cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("my-advances")]
        public async Task<IActionResult> GetAdvancesByUser(CancellationToken cancellationToken)
        {
            var userId = identityService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }
            var result = await advanceService.GetAdvancesByUserIdAsync(userId, cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAdvancesByUser(string userId, CancellationToken cancellationToken)
        {
            var result = await advanceService.GetAdvancesByUserIdAsync(userId, cancellationToken);
            return result.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpPut("{advanceId}/complete")]
        public async Task<IActionResult> CompleteAdvance(string advanceId, [FromBody] ApproveAdvanceDTO? approveAdvanceDTO = null)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var result = await advanceService.CompleteAdvanceAsync(advanceId, approveAdvanceDTO);
            return result.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpPut("{advanceId}/approve")]
        public async Task<IActionResult> ApproveAdvance(string advanceId, [FromBody] ApproveAdvanceDTO? approveAdvanceDTO = null)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var result = await advanceService.ApproveAdvanceAsync(advanceId, approveAdvanceDTO);
            return result.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpPut("{advanceId}/reject")]
        public async Task<IActionResult> RejectAdvance(string advanceId, [FromQuery] string? rejectReason)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }
            var result = await advanceService.RejectAdvanceAsync(advanceId, rejectReason);
            return result.ToActionResult();
        }

        [HttpGet("{advanceId}")]
        public async Task<IActionResult> GetAdvanceById(string advanceId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var result = await advanceService.GetAdvanceByIdAsync(advanceId, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdvance([FromBody] CreateAdvanceDTO createAdvanceDTO, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await advanceService.CreateAdvanceAsync(createAdvanceDTO, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPut("{advanceId}")]
        [Authorize(Policy = "FinansPolicy")]
        public async Task<IActionResult> UpdateAdvance(string advanceId, [FromBody] UpdateAdvanceDTO updateAdvanceDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await advanceService.UpdateAdvanceAsync(advanceId, updateAdvanceDTO, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{advanceId}")]
        [Authorize(Policy = "FinansPolicy")]
        public async Task<IActionResult> DeleteAdvance(string advanceId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var result = await advanceService.DeleteAdvanceAsync(advanceId, cancellationToken);
            return result.ToActionResult();
        }
    }
}