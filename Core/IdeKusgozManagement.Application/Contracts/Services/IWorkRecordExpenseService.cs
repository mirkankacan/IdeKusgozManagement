using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IWorkRecordExpenseService
    {
        Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchCreateWorkRecordExpensesAsync(IEnumerable<CreateWorkRecordExpenseDTO> createWorkRecordExpenseDTOs, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchUpdateWorkRecordExpensesAsync(IEnumerable<CreateWorkRecordExpenseDTO> createWorkRecordExpenseDTOs, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> BatchDeleteWorkRecordExpensesAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}