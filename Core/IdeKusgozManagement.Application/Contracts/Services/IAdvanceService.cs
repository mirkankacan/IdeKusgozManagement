using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AdvanceDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IAdvanceService
    {
        Task<ServiceResult<AdvanceStatisticDTO>> GetAdvanceStatisticsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<AdvanceDTO>>> GetAdvancesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<AdvanceDTO>>> GetApprovedAdvancesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<AdvanceDTO>>> GetCompletedAdvancesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<AdvanceDTO>> GetAdvanceByIdAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ServiceResult<string>> CreateAdvanceAsync(CreateAdvanceDTO createAdvanceDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> UpdateAdvanceAsync(string advanceId, UpdateAdvanceDTO updateAdvanceDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteAdvanceAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<AdvanceDTO>>> GetAdvancesByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> ApproveAdvanceAsync(string advanceId, ApproveAdvanceDTO? approveAdvanceDTO = null, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> RejectAdvanceAsync(string advanceId, string? rejectReason, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> CompleteAdvanceAsync(string advanceId, ApproveAdvanceDTO? approveAdvanceDTO = null, CancellationToken cancellationToken = default);
    }
}