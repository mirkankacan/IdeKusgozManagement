using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IWorkRecordExpenseService
    {
        Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchCreateOrModifyWorkRecordExpensesAsync(IEnumerable<CreateOrModifyWorkRecordExpenseDTO> expenseDTOs, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> BatchDeleteWorkRecordExpensesAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchUpdateWorkRecordExpensesAsync(IEnumerable<UpdateWorkRecordExpenseDTO> expenseDTOs, CancellationToken cancellationToken = default);
    }
}