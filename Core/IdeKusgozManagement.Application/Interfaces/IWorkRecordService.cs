using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IWorkRecordService
    {
        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> GetAllWorkRecordsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordDTO>> GetWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> GetWorkRecordsByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> GetWorkRecordsByDateAndUserAsync(DateTime date, string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordDTO>>> BatchCreateWorkRecordsAsync(List<CreateWorkRecordDTO> createWorkRecordDTOs, CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordDTO>> UpdateWorkRecordAsync(string id, UpdateWorkRecordDTO updateWorkRecordDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ApproveWorkRecordAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> RejectWorkRecordAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<int>> GetWorkRecordCountByStatusAsync(WorkRecordStatus status, CancellationToken cancellationToken = default);

        Task<ApiResponse<int>> GetWorkRecordCountAsync(CancellationToken cancellationToken = default);
    }
}