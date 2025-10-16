using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.LeaveRequestModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface ILeaveRequestApiService
    {
        Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestViewModel>> GetLeaveRequestByIdAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestViewModel>> CreateLeaveRequestAsync(CreateLeaveRequestViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsByStatusAsync(int status, string? userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ApproveLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> RejectLeaveRequestAsync(string leaveRequestId, string? rejectReason, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetMyLeaveRequestsAsync(CancellationToken cancellationToken = default);
    }
}