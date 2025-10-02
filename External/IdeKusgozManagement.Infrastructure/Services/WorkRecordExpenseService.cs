using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class WorkRecordExpenseService(IUnitOfWork unitOfWork, IFileService fileService) : IWorkRecordExpenseService
    {
        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchCreateWorkRecordExpensesAsync(IEnumerable<CreateWorkRecordExpenseDTO> createWorkRecordExpenseDTOs, CancellationToken cancellationToken = default)
        {
            var mappedDTOs = createWorkRecordExpenseDTOs.Adapt<IEnumerable<IdtWorkRecordExpense>>();
            await unitOfWork.GetRepository<IdtWorkRecordExpense>().AddRangeAsync(mappedDTOs, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return ApiResponse<IEnumerable<WorkRecordExpenseDTO>>.Success(mappedDTOs.Adapt<IEnumerable<WorkRecordExpenseDTO>>());
        }

        public Task<ApiResponse<bool>> BatchDeleteWorkRecordExpensesAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<WorkRecordExpenseDTO>>> BatchUpdateWorkRecordExpensesAsync(IEnumerable<CreateWorkRecordExpenseDTO> createWorkRecordExpenseDTOs, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}