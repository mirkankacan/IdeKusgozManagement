using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.AdvanceDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdvancesController(IAdvanceService advanceService, IIdentityService identityService) : ControllerBase
    {
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpGet]
        public async Task<IActionResult> GetAdvances(CancellationToken cancellationToken = default)
        {
            var result = await advanceService.GetAdvancesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [RoleFilter("Admin", "Yönetici")]
        [HttpGet("chief-processed")]
        public async Task<IActionResult> GetChiefProcessedAdvances(CancellationToken cancellationToken = default)
        {
            var result = await advanceService.GetChiefProcessedAdvancesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("my-advances")]
        public async Task<IActionResult> GetAdvancesByUser(CancellationToken cancellationToken = default)
        {
            var userId = identityService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }
            var result = await advanceService.GetAdvancesByUserIdAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAdvancesByUser(string userId, CancellationToken cancellationToken = default)
        {
            var result = await advanceService.GetAdvancesByUserIdAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpPut("{advanceId}/approve")]
        public async Task<IActionResult> ApproveAdvance(string advanceId)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var result = await advanceService.ApproveAdvanceAsync(advanceId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpPut("{advanceId}/reject")]
        public async Task<IActionResult> RejectAdvance(string advanceId, [FromQuery] string? rejectReason)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }
            var result = await advanceService.RejectAdvanceAsync(advanceId, rejectReason);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{advanceId}")]
        public async Task<IActionResult> GetAdvanceById(string advanceId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var result = await advanceService.GetAdvanceByIdAsync(advanceId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdvance([FromBody] CreateAdvanceDTO createAdvanceDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await advanceService.CreateAdvanceAsync(createAdvanceDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{advanceId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> UpdateAdvance(string advanceId, [FromBody] UpdateAdvanceDTO updateAdvanceDTO, CancellationToken cancellationToken = default)
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
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{advanceId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> DeleteAdvance(string advanceId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(advanceId))
            {
                return BadRequest("Avans ID'si gereklidir");
            }

            var result = await advanceService.DeleteAdvanceAsync(advanceId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}