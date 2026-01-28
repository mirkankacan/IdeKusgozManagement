using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface ILeaveRequestService
    {
        Task<ServiceResult<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<LeaveRequestDTO>> GetLeaveRequestByIdAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ServiceResult<LeaveRequestDTO>> CreateLeaveRequestAsync(CreateLeaveRequestDTO createLeaveRequestDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> UpdateLeaveRequestAsync(string leaveRequestId, UpdateLeaveRequestDTO updateLeaveRequestDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByStatusAsync(LeaveRequestStatus status, string? userId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<LeaveRequestDTO>>> GetSubordinateLeaveRequestsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> ApproveLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> RejectLeaveRequestAsync(string leaveRequestId, string? rejectReason, CancellationToken cancellationToken = default);
    }
}