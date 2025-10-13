using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.AdvanceModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IAdvanceApiService
    {
        Task<ApiResponse<IEnumerable<AdvanceViewModel>>> GetAdvancesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<AdvanceViewModel>>> GetChiefProcessedAdvancesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<AdvanceViewModel>>> GetMyAdvancesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<AdvanceViewModel>> GetAdvanceByIdAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateAdvanceAsync(CreateAdvanceViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateAdvanceAsync(string advanceId, UpdateAdvanceViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteAdvanceAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<AdvanceViewModel>>> GetAdvancesByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ApproveAdvanceAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> RejectAdvanceAsync(string advanceId, string? rejectReason, CancellationToken cancellationToken = default);
    }
}