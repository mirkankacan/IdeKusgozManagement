using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IWorkRecordService
    {
        Task<ServiceResult<IEnumerable<WorkRecordDTO>>> GetWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<WorkRecordDTO>>> GetWorkRecordsByUserIdDateStatusAsync(string userId, DateTime date, WorkRecordStatus status, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<WorkRecordDTO>>> GetApprovedWorkRecordsByUserAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<WorkRecordDTO>>> BatchCreateOrModifyWorkRecordsAsync(IEnumerable<CreateOrModifyWorkRecordDTO> workRecordDTOs, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<WorkRecordDTO>>> BatchUpdateWorkRecordsByUserIdAsync(string userId, IEnumerable<CreateOrModifyWorkRecordDTO> workRecordDTOs, CancellationToken cancellationToken = default);

        Task<ServiceResult<WorkRecordDTO>> ApproveWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ServiceResult<WorkRecordDTO>> RejectWorkRecordByIdAsync(string id, string? rejectReason, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<WorkRecordDTO>>> BatchApproveWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<WorkRecordDTO>>> BatchRejectWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default);
    }
}