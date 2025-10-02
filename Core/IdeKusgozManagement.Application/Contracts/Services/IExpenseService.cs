using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IExpenseService
    {
        Task<ApiResponse<IEnumerable<ExpenseDTO>>> GetExpensesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<ExpenseDTO>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateExpenseAsync(CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateExpenseAsync(string expenseId, UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<ExpenseDTO>>> GetActiveExpensesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> EnableExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DisableExpenseAsync(string expenseId, CancellationToken cancellationToken = default);
    }
}