using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IExpenseService
    {
        Task<ServiceResponse<IEnumerable<ExpenseDTO>>> GetExpensesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<ExpenseDTO>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<string>> CreateExpenseAsync(CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> UpdateExpenseAsync(string expenseId, UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<ExpenseDTO>>> GetActiveExpensesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> EnableExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DisableExpenseAsync(string expenseId, CancellationToken cancellationToken = default);
    }
}