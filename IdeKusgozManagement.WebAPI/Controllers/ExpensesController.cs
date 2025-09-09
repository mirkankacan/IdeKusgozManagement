using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        /// <summary>
        /// Tüm masraf türlerini getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetExpenses(CancellationToken cancellationToken = default)
        {
            var result = await _expenseService.GetExpensesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Aktif tüm masraf türlerini getirir
        /// </summary>
        [HttpGet("active-expenses")]
        public async Task<IActionResult> GetActiveExpenses(CancellationToken cancellationToken = default)
        {
            var result = await _expenseService.GetActiveExpensesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Masraf türünü aktifleştirir
        /// </summary>
        /// <param name="expenseId">Masraf ID'si</param>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpPut("{expenseId}/enable")]
        public async Task<IActionResult> EnableExpense(string expenseId)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }
            var result = await _expenseService.EnableExpenseAsync(expenseId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Masraf türünü pasifleştirir
        /// </summary>
        /// <param name="expenseId">Masraf ID'si</param>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpPut("{expenseId}/disable")]
        public async Task<IActionResult> DisableExpense(string expenseId)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }
            var result = await _expenseService.DisableExpenseAsync(expenseId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre masraf türü getirir
        /// </summary>
        /// <param name="expenseId">Masraf türü ID'si</param>
        [HttpGet("{expenseId}")]
        public async Task<IActionResult> GetExpenseById(string expenseId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var result = await _expenseService.GetExpenseByIdAsync(expenseId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni masraf türü oluşturur
        /// </summary>
        /// <param name="createExpenseDTO">Masraf türü bilgileri</param>
        [HttpPost]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _expenseService.CreateExpenseAsync(createExpenseDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Masraf türü günceller
        /// </summary>
        /// <param name="expenseId">Masraf türü ID'si</param>
        /// <param name="updateExpenseDTO">Güncellenecek masraf türü bilgileri</param>
        [HttpPut("{expenseId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> UpdateExpense(string expenseId, [FromBody] UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _expenseService.UpdateExpenseAsync(expenseId, updateExpenseDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Masraf türü siler
        /// </summary>
        /// <param name="expenseId">Masraf türü ID'si</param>
        [HttpDelete("{expenseId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> DeleteExpense(string expenseId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var result = await _expenseService.DeleteExpenseAsync(expenseId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}