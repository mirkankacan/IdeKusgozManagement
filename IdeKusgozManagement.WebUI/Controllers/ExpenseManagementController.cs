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

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("liste")]
        public async Task<IActionResult> GetExpenses(CancellationToken cancellationToken = default)
        {
            var response = await _expenseApiService.GetAllExpensesAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("aktif-liste")]
        public async Task<IActionResult> GetActiveExpenses(CancellationToken cancellationToken = default)
        {
            var response = await _expenseApiService.GetAllActiveExpensesAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpenseById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var response = await _expenseApiService.GetExpenseByIdAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
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

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(string id, [FromBody] UpdateExpenseViewModel model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _expenseApiService.UpdateExpenseAsync(id, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var response = await _expenseApiService.DeleteExpenseAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("{id}/aktif-et")]
        public async Task<IActionResult> ActivateExpense(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var response = await _expenseApiService.ActivateExpenseAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("{id}/pasif-et")]
        public async Task<IActionResult> DeactivateExpense(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var response = await _expenseApiService.DeactivateExpenseAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
