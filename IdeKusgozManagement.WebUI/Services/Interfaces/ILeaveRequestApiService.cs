using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.LeaveRequestModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface ILeaveRequestApiService
    {
        Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsByStatusAsync(int status, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsByUserIdAndStatusAsync(string userId, int status, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetMyLeaveRequestsByStatusAsync(int status, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetMyLeaveRequestsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestViewModel>> ApproveLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestViewModel>> RejectLeaveRequestAsync(string leaveRequestId, string? rejectReason, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestViewModel>> GetLeaveRequestByIdAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestViewModel>> CreateLeaveRequestAsync(CreateLeaveRequestViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default);
    }
}