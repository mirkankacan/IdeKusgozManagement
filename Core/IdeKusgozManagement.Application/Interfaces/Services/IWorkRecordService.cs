using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IWorkRecordService
    {
        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> GetWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchCreateWorkRecordsAsync(IEnumerable<CreateWorkRecordDTO> createWorkRecordDTOs, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchUpdateWorkRecordsByUserIdAsync(string userId, IEnumerable<UpdateWorkRecordDTO> updateWorkRecordDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> BatchApproveWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> BatchRejectWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);
    }
}