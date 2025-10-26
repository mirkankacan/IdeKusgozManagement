using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface ILeaveRequestService
    {
        Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestDTO>> GetLeaveRequestByIdAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestDTO>> CreateLeaveRequestAsync(CreateLeaveRequestDTO createLeaveRequestDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateLeaveRequestAsync(string leaveRequestId, UpdateLeaveRequestDTO updateLeaveRequestDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByStatusAsync(LeaveRequestStatus status, string? userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ApproveLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> RejectLeaveRequestAsync(string leaveRequestId, string? rejectReason, CancellationToken cancellationToken = default);
    }
}