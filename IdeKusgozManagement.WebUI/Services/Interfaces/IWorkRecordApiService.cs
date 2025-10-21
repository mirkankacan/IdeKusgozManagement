using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.WorkRecordModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IWorkRecordApiService
    {
        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetMyWorkRecordsByDateAsync(DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchCreateOrModifyWorkRecordsAsync(IEnumerable<CreateOrModifyWorkRecordViewModel> createWorkRecordViewModels, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchUpdateWorkRecordsByUserIdAsync(string userId, IEnumerable<CreateOrModifyWorkRecordViewModel> updateWorkRecordViewModel, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchApproveWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchRejectWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordViewModel>> ApproveWorkRecordByIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<WorkRecordViewModel>> RejectWorkRecordByIdAsync(string userId, string? rejectReason, CancellationToken cancellationToken = default);
    }
}