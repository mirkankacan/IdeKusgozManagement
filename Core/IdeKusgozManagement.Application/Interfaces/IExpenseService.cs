using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<ApiResponse<IEnumerable<ExpenseDTO>>> GetAllExpensesAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<IEnumerable<ExpenseDTO>>> GetAllActiveExpensesAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> ActivateExpenseAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeactivateExpenseAsync(string id, CancellationToken cancellationToken = default);
        Task<ApiResponse<ExpenseDTO>> GetExpenseByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateExpenseAsync(CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateExpenseAsync(string id, UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteExpenseAsync(string id, CancellationToken cancellationToken = default);
    }
}