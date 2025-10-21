using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IWorkRecordService
    {
        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> GetWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchCreateOrModifyWorkRecordsAsync(IEnumerable<CreateModifyWorkRecordDTO> createWorkRecordDTOs, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchUpdateWorkRecordsByUserIdAsync(string userId, IEnumerable<UpdateWorkRecordDTO> updateWorkRecordDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordDTO>> ApproveWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordDTO>> RejectWorkRecordByIdAsync(string id, string? rejectReason, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchApproveWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchRejectWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default);
    }
}