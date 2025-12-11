using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.MachineWorkRecordDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IMachineWorkRecordService
    {
        Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> GetMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> GetMachineWorkRecordsByUserIdDateStatusAsync(string userId, DateTime date, WorkRecordStatus status, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> GetApprovedMachineWorkRecordsByUserAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> BatchCreateOrModifyMachineWorkRecordsAsync(IEnumerable<CreateOrModifyMachineWorkRecordDTO> workRecordDTOs, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> BatchUpdateMachineWorkRecordsByUserIdAsync(string userId, IEnumerable<CreateOrModifyMachineWorkRecordDTO> workRecordDTOs, CancellationToken cancellationToken = default);

        Task<ServiceResponse<MachineWorkRecordDTO>> ApproveMachineWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ServiceResponse<MachineWorkRecordDTO>> RejectMachineWorkRecordByIdAsync(string id, string? rejectReason, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> BatchApproveMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<MachineWorkRecordDTO>>> BatchRejectMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default);
    }
}