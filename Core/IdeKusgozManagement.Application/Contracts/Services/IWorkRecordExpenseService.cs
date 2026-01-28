using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IWorkRecordExpenseService
    {
        Task<ServiceResult<IEnumerable<WorkRecordExpenseDTO>>> BatchCreateOrModifyWorkRecordExpensesAsync(IEnumerable<CreateOrModifyWorkRecordExpenseDTO> expenseDTOs, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<WorkRecordExpenseDTO>>> BatchUpdateWorkRecordByUserIdExpensesAsync(string userId, IEnumerable<CreateOrModifyWorkRecordExpenseDTO> expenseDTOs, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> BatchDeleteWorkRecordExpensesAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}