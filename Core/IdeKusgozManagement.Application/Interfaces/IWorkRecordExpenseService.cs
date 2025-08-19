using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IWorkRecordExpenseService
    {
        Task<ApiResponse<WorkRecordExpenseDTO>> AddExpenseToWorkRecordAsync(string workRecordId, CreateWorkRecordExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> GetWorkRecordExpensesAsync(string workRecordId, CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordExpenseDTO>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordExpenseDTO>> UpdateWorkRecordExpenseAsync(string expenseId, UpdateWorkRecordExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteWorkRecordExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> GetAllExpensesByUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<decimal>> GetTotalExpenseAmountByWorkRecordAsync(string workRecordId, CancellationToken cancellationToken = default);

        Task<ApiResponse<decimal>> GetTotalExpenseAmountByUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<decimal>> GetTotalExpenseAmountAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<decimal>> GetAverageExpenseAmountAsync(CancellationToken cancellationToken = default);
    }
}