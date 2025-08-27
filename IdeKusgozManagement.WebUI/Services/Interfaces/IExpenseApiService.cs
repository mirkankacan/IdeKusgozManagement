using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.ExpenseModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IExpenseApiService
    {
        Task<ApiResponse<IEnumerable<ExpenseViewModel>>> GetAllExpensesAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ExpenseViewModel>>> GetAllActiveExpensesAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<ExpenseViewModel>> GetExpenseByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<ApiResponse<string>> CreateExpenseAsync(CreateExpenseViewModel model, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> UpdateExpenseAsync(string id, UpdateExpenseViewModel model, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteExpenseAsync(string id, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> ActivateExpenseAsync(string id, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeactivateExpenseAsync(string id, CancellationToken cancellationToken = default);
    }
}
