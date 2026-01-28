using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Enums;
using IdeKusgozManagement.Infrastructure.Authorization;
using IdeKusgozManagement.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController(IExpenseService expenseService) : ControllerBase
    {
        /// <summary>
        /// Tüm masraf türlerini getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetExpenses(CancellationToken cancellationToken)
        {
            var result = await expenseService.GetExpensesAsync(cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Aktif tüm masraf türlerini getirir
        /// </summary>
        [HttpGet("active-expenses")]
        public async Task<IActionResult> GetActiveExpenses(CancellationToken cancellationToken)
        {
            var result = await expenseService.GetActiveExpensesAsync(cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// ExpenseType'a göre masraf türlerini getirir
        /// </summary>
        /// <param name="expenseType">Masraf türü (ExpenseItem = 0, InvoiceItem = 1)</param>
        [HttpGet("by-type/{expenseType}")]
        public async Task<IActionResult> GetExpensesByType(ExpenseType expenseType, CancellationToken cancellationToken)
        {
            var result = await expenseService.GetExpensesByTypeAsync(expenseType, cancellationToken);
            return result.ToActionResult();
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
            var result = await expenseService.EnableExpenseAsync(expenseId);
            return result.ToActionResult();
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
            var result = await expenseService.DisableExpenseAsync(expenseId);
            return result.ToActionResult();
        }

        /// <summary>
        /// ID'ye göre masraf türü getirir
        /// </summary>
        /// <param name="expenseId">Masraf türü ID'si</param>
        [HttpGet("{expenseId}")]
        public async Task<IActionResult> GetExpenseById(string expenseId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var result = await expenseService.GetExpenseByIdAsync(expenseId, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Yeni masraf türü oluşturur
        /// </summary>
        /// <param name="createExpenseDTO">Masraf türü bilgileri</param>
        [HttpPost]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await expenseService.CreateExpenseAsync(createExpenseDTO, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Masraf türü günceller
        /// </summary>
        /// <param name="expenseId">Masraf türü ID'si</param>
        /// <param name="updateExpenseDTO">Güncellenecek masraf türü bilgileri</param>
        [HttpPut("{expenseId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> UpdateExpense(string expenseId, [FromBody] UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await expenseService.UpdateExpenseAsync(expenseId, updateExpenseDTO, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Masraf türü siler
        /// </summary>
        /// <param name="expenseId">Masraf türü ID'si</param>
        [HttpDelete("{expenseId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> DeleteExpense(string expenseId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var result = await expenseService.DeleteExpenseAsync(expenseId, cancellationToken);
            return result.ToActionResult();
        }
    }
}