using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AdvanceDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IAdvanceService
    {
        Task<ApiResponse<IEnumerable<AdvanceDTO>>> GetAdvancesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<AdvanceDTO>>> GetChiefProcessedAdvancesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<AdvanceDTO>> GetAdvanceByIdAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateAdvanceAsync(CreateAdvanceDTO createAdvanceDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateAdvanceAsync(string advanceId, UpdateAdvanceDTO updateAdvanceDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteAdvanceAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<AdvanceDTO>>> GetAdvancesByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ApproveAdvanceAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> RejectAdvanceAsync(string advanceId, string? rejectReason, CancellationToken cancellationToken = default);
    }
}