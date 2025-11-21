using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IWorkRecordExpenseService
    {
        Task<ServiceResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchCreateOrModifyWorkRecordExpensesAsync(IEnumerable<CreateOrModifyWorkRecordExpenseDTO> expenseDTOs, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchUpdateWorkRecordByUserIdExpensesAsync(string userId, IEnumerable<CreateOrModifyWorkRecordExpenseDTO> expenseDTOs, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> BatchDeleteWorkRecordExpensesAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}