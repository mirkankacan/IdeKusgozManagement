using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces;
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
        public async Task<IActionResult> GetAllExpenses(CancellationToken cancellationToken = default)
        {
            var result = await _expenseService.GetAllExpensesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Aktif tüm masraf türlerini getirir
        /// </summary>
        [HttpGet("active-expenses")]
        public async Task<IActionResult> GetAllActiveExpenses(CancellationToken cancellationToken = default)
        {
            var result = await _expenseService.GetAllActiveExpensesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Masraf türünü aktifleştirir
        /// </summary>
        /// <param name="id">Ekipman ID'si</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateExpense(string id)
        {
            var result = await _expenseService.ActivateExpenseAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Masraf türünü pasifleştirir
        /// </summary>
        /// <param name="id">Ekipman ID'si</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateExpense(string id)
        {
            var result = await _expenseService.DeactivateExpenseAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre masraf türü getirir
        /// </summary>
        /// <param name="id">Masraf türü ID'si</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpenseById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var result = await _expenseService.GetExpenseByIdAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni masraf türü oluşturur
        /// </summary>
        /// <param name="createExpenseDTO">Masraf türü bilgileri</param>
        [HttpPost]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _expenseService.CreateExpenseAsync(createExpenseDTO, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(GetExpenseById), new { id = result.Data }, result) : BadRequest(result);
        }

        /// <summary>
        /// Masraf türü günceller
        /// </summary>
        /// <param name="id">Masraf türü ID'si</param>
        /// <param name="updateExpenseDTO">Güncellenecek masraf türü bilgileri</param>
        [HttpPut("{id}")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> UpdateExpense(string id, [FromBody] UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _expenseService.UpdateExpenseAsync(id, updateExpenseDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Masraf türü siler
        /// </summary>
        /// <param name="id">Masraf türü ID'si</param>
        [HttpDelete("{id}")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> DeleteExpense(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var result = await _expenseService.DeleteExpenseAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}