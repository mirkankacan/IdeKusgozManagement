using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetAllLeaveRequestsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetAllActiveLeaveRequestsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ApproveLeaveRequestAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> RejectLeaveRequestAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<LeaveRequestDTO>> GetLeaveRequestByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateLeaveRequestAsync(CreateLeaveRequestDTO createLeaveRequestDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateLeaveRequestAsync(string id, UpdateLeaveRequestDTO updateLeaveRequestDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteLeaveRequestAsync(string id, CancellationToken cancellationToken = default);
    }
}