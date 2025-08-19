using System.Security.Claims;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkRecordExpensesController : ControllerBase
    {
        private readonly IWorkRecordExpenseService _workRecordExpenseService;

        public WorkRecordExpensesController(IWorkRecordExpenseService workRecordExpenseService)
        {
            _workRecordExpenseService = workRecordExpenseService;
        }

        /// <summary>
        /// İş kaydına masraf ekler
        /// </summary>
        /// <param name="workRecordId">İş kaydı ID'si</param>
        /// <param name="createExpenseDTO">Masraf bilgileri</param>
        [HttpPost("work-record/{workRecordId}")]
        public async Task<IActionResult> AddExpenseToWorkRecord(
            string workRecordId,
            [FromBody] CreateWorkRecordExpenseDTO createExpenseDTO,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _workRecordExpenseService.AddExpenseToWorkRecordAsync(workRecordId, createExpenseDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İş kaydının masraflarını getirir
        /// </summary>
        /// <param name="workRecordId">İş kaydı ID'si</param>
        [HttpGet("work-record/{workRecordId}")]
        public async Task<IActionResult> GetWorkRecordExpenses(
            string workRecordId,
            CancellationToken cancellationToken = default)
        {
            var result = await _workRecordExpenseService.GetWorkRecordExpensesAsync(workRecordId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre masraf kaydı getirir
        /// </summary>
        /// <param name="expenseId">Masraf ID'si</param>
        [HttpGet("{expenseId}")]
        public async Task<IActionResult> GetExpenseById(
            string expenseId,
            CancellationToken cancellationToken = default)
        {
            var result = await _workRecordExpenseService.GetExpenseByIdAsync(expenseId);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Masraf kaydını günceller
        /// </summary>
        /// <param name="expenseId">Masraf ID'si</param>
        /// <param name="updateWorkRecordExpenseDTO">Güncellenecek masraf bilgileri</param>
        [HttpPut("{expenseId}")]
        public async Task<IActionResult> UpdateWorkRecordExpense(
            string expenseId,
            [FromBody] UpdateWorkRecordExpenseDTO updateWorkRecordExpenseDTO,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _workRecordExpenseService.UpdateWorkRecordExpenseAsync(expenseId, updateWorkRecordExpenseDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Masraf kaydını siler
        /// </summary>
        /// <param name="expenseId">Masraf ID'si</param>
        [HttpDelete("{expenseId}")]
        public async Task<IActionResult> DeleteWorkRecordExpense(
            string expenseId,
            CancellationToken cancellationToken = default)
        {
            var result = await _workRecordExpenseService.DeleteWorkRecordExpenseAsync(expenseId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcının tüm masraflarını getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        [HttpGet("user/{userId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetAllExpensesByUser(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var result = await _workRecordExpenseService.GetAllExpensesByUserAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Mevcut kullanıcının tüm masraflarını getirir
        /// </summary>
        [HttpGet("my-expenses")]
        public async Task<IActionResult> GetMyExpenses(CancellationToken cancellationToken = default)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }

            var result = await _workRecordExpenseService.GetAllExpensesByUserAsync(currentUserId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İş kaydının toplam masraf tutarını getirir
        /// </summary>
        /// <param name="workRecordId">İş kaydı ID'si</param>
        [HttpGet("work-record/{workRecordId}/total")]
        public async Task<IActionResult> GetTotalExpenseAmountByWorkRecord(
            string workRecordId,
            CancellationToken cancellationToken = default)
        {
            var result = await _workRecordExpenseService.GetTotalExpenseAmountByWorkRecordAsync(workRecordId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Toplam masraf tutarını getirir
        /// </summary>
        [HttpGet("total")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetTotalExpenseAmount(
            string workRecordId,
            CancellationToken cancellationToken = default)
        {
            var result = await _workRecordExpenseService.GetTotalExpenseAmountAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}