using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.WorkRecordModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IWorkRecordApiService
    {
        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetAllWorkRecordsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordViewModel>> GetWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetMyWorkRecordsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetWorkRecordsByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetWorkRecordsByDateAndUserAsync(DateTime date, string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchCreateWorkRecordsAsync(List<CreateWorkRecordViewModel> createWorkRecordViewModels, CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordViewModel>> UpdateWorkRecordAsync(string id, UpdateWorkRecordViewModel updateWorkRecordViewModel, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> BatchApproveWorkRecordsByUserAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> BatchRejectWorkRecordsByUserAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<int>> GetWorkRecordCountAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<int>> GetWorkRecordCountByStatusAsync(int status, CancellationToken cancellationToken = default);
    }
}