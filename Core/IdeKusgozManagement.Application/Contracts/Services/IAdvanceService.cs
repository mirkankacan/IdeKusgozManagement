using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AdvanceDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IAdvanceService
    {
        Task<ServiceResponse<IEnumerable<AdvanceDTO>>> GetAdvancesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<AdvanceDTO>>> GetChiefProcessedAdvancesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<AdvanceDTO>> GetAdvanceByIdAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<string>> CreateAdvanceAsync(CreateAdvanceDTO createAdvanceDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> UpdateAdvanceAsync(string advanceId, UpdateAdvanceDTO updateAdvanceDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteAdvanceAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<AdvanceDTO>>> GetAdvancesByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> ApproveAdvanceAsync(string advanceId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> RejectAdvanceAsync(string advanceId, string? rejectReason, CancellationToken cancellationToken = default);
    }
}