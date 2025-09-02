using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Application.Interfaces;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        public Task<ApiResponse<bool>> ApproveLeaveRequestAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<string>> CreateLeaveRequestAsync(CreateLeaveRequestDTO createLeaveRequestDTO, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteLeaveRequestAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetAllActiveLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<LeaveRequestDTO>>> GetAllLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<LeaveRequestDTO>> GetLeaveRequestByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> RejectLeaveRequestAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> UpdateLeaveRequestAsync(string id, UpdateLeaveRequestDTO updateLeaveRequestDTO, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}