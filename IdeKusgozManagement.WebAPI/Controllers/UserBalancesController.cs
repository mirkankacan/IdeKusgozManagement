using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.UserBalanceDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserBalancesController(IUserBalanceService userBalanceService, IIdentityService identityService) : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserBalanceByUser(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }

            var result = await userBalanceService.GetUserBalancesByUserAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("my-balance")]
        public async Task<IActionResult> GetMyBalances(CancellationToken cancellationToken)
        {
            var userId = identityService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }

            var result = await userBalanceService.GetUserBalancesByUserAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{userId}/decrease")]
        public async Task<IActionResult> DecreaseUserBalance(string userId, UpdateUserBalanceDTO updateUserBalanceDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await userBalanceService.DecreaseUserBalanceAsync(userId, updateUserBalanceDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{userId}/increase")]
        public async Task<IActionResult> IncreaseUserBalance(string userId, UpdateUserBalanceDTO updateUserBalanceDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await userBalanceService.IncreaseUserBalanceAsync(userId, updateUserBalanceDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}