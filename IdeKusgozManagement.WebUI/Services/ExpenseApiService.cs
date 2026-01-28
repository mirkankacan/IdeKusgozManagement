using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.ExpenseModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class ExpenseApiService : IExpenseApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ExpenseApiService> _logger;
        private const string BaseEndpoint = "api/expenses";

        public ExpenseApiService(
            IApiService apiService,
            ILogger<ExpenseApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<ExpenseViewModel>>> GetExpensesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<ExpenseViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<ExpenseViewModel>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<ExpenseViewModel>($"{BaseEndpoint}/{expenseId}", cancellationToken);
        }

        public async Task<ApiResponse<string>> CreateExpenseAsync(CreateExpenseViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<string>(BaseEndpoint, model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> UpdateExpenseAsync(string expenseId, UpdateExpenseViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{expenseId}", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{expenseId}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<ExpenseViewModel>>> GetActiveExpensesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<ExpenseViewModel>>($"{BaseEndpoint}/active-expenses", cancellationToken);
        }

        public async Task<ApiResponse<bool>> EnableExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{expenseId}/enable", null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DisableExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{expenseId}/disable", null, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<ExpenseViewModel>>> GetExpensesByTypeAsync(int expenseType, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<ExpenseViewModel>>($"{BaseEndpoint}/by-type/{expenseType}", cancellationToken);
        }
    }
}
