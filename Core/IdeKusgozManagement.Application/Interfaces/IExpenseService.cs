using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<ApiResponse<IEnumerable<ExpenseListDTO>>> GetAllExpensesAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<ExpenseDetailDTO>> GetExpenseByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<ApiResponse<string>> CreateExpenseAsync(CreateExpenseDTO createExpenseDTO, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> UpdateExpenseAsync(string id, UpdateExpenseDTO updateExpenseDTO, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteExpenseAsync(string id, CancellationToken cancellationToken = default);
    }
}
