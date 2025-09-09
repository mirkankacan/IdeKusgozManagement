using IdeKusgozManagement.WebUI.Models.ExpenseModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("masraf-yonetimi")]
    public class ExpenseManagementController : Controller
    {
        private readonly IExpenseApiService _expenseApiService;

        public ExpenseManagementController(IExpenseApiService expenseApiService)
        {
            _expenseApiService = expenseApiService;
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetExpenses(CancellationToken cancellationToken = default)
        {
            var response = await _expenseApiService.GetExpensesAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("aktif-liste")]
        public async Task<IActionResult> GetActiveExpenses(CancellationToken cancellationToken = default)
        {
            var response = await _expenseApiService.GetActiveExpensesAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("{expenseId}")]
        public async Task<IActionResult> GetExpenseById(string expenseId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var response = await _expenseApiService.GetExpenseByIdAsync(expenseId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpPost("")]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _expenseApiService.CreateExpenseAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpPut("{expenseId}")]
        public async Task<IActionResult> UpdateExpense(string expenseId, [FromBody] UpdateExpenseViewModel model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _expenseApiService.UpdateExpenseAsync(expenseId, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpDelete("{expenseId}")]
        public async Task<IActionResult> DeleteExpense(string expenseId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var response = await _expenseApiService.DeleteExpenseAsync(expenseId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpPut("{expenseId}/aktif-et")]
        public async Task<IActionResult> EnableExpense(string expenseId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var response = await _expenseApiService.EnableExpenseAsync(expenseId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpPut("{expenseId}/pasif-et")]
        public async Task<IActionResult> DisableExpense(string expenseId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var response = await _expenseApiService.DisableExpenseAsync(expenseId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}