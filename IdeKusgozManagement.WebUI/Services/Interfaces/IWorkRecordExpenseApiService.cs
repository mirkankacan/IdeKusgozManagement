using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.WorkRecordExpenseModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IWorkRecordExpenseApiService
    {
        Task<ApiResponse<WorkRecordExpenseViewModel>> AddExpenseToWorkRecordAsync(string workRecordId, CreateWorkRecordExpenseViewModel createExpenseViewModel, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordExpenseViewModel>>> GetWorkRecordExpensesAsync(string workRecordId, CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordExpenseViewModel>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordExpenseViewModel>> UpdateWorkRecordExpenseAsync(string expenseId, UpdateWorkRecordExpenseViewModel updateExpenseViewModel, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteWorkRecordExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordExpenseViewModel>>> GetAllExpensesByUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordExpenseViewModel>>> GetMyExpensesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<decimal>> GetTotalExpenseAmountByWorkRecordAsync(string workRecordId, CancellationToken cancellationToken = default);

        Task<ApiResponse<decimal>> GetTotalExpenseAmountByUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<decimal>> GetMyTotalExpenseAmountAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<decimal>> GetTotalExpenseAmountAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<decimal>> GetAverageExpenseAmountAsync(CancellationToken cancellationToken = default);
    }
}