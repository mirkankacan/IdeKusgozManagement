using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.ExpenseModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IExpenseApiService
    {
        Task<ApiResponse<IEnumerable<ExpenseViewModel>>> GetExpensesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<ExpenseViewModel>>> GetActiveExpensesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> EnableExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DisableExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<ExpenseViewModel>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateExpenseAsync(CreateExpenseViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateExpenseAsync(string expenseId, UpdateExpenseViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteExpenseAsync(string expenseId, CancellationToken cancellationToken = default);
    }
}