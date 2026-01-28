using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IExpenseService
    {
        Task<ServiceResult<IEnumerable<ExpenseDTO>>> GetExpensesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<ExpenseDTO>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ServiceResult<string>> CreateExpenseAsync(CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> UpdateExpenseAsync(string expenseId, UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<ExpenseDTO>>> GetActiveExpensesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> EnableExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DisableExpenseAsync(string expenseId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<ExpenseDTO>>> GetExpensesByTypeAsync(Domain.Enums.ExpenseType expenseType, CancellationToken cancellationToken = default);
    }
}