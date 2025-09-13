using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface ILeaveRequestService
    {
        Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByStatusAsync(LeaveRequestStatus status, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByUserIdAndStatusAsync(string userId, LeaveRequestStatus status, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestDTO>> ApproveLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestDTO>> RejectLeaveRequestAsync(string leaveRequestId, string? rejectReason, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestDTO>> GetLeaveRequestByIdAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestDTO>> CreateLeaveRequestAsync(CreateLeaveRequestDTO createLeaveRequestDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default);
    }
}