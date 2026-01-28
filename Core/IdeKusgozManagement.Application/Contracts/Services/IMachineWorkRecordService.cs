using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.MachineWorkRecordDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IMachineWorkRecordService
    {
        Task<ServiceResult<IEnumerable<MachineWorkRecordDTO>>> GetMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<MachineWorkRecordDTO>>> GetMachineWorkRecordsByUserIdDateStatusAsync(string userId, DateTime date, WorkRecordStatus status, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<MachineWorkRecordDTO>>> GetApprovedMachineWorkRecordsByUserAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<MachineWorkRecordDTO>>> BatchCreateOrModifyMachineWorkRecordsAsync(IEnumerable<CreateOrModifyMachineWorkRecordDTO> workRecordDTOs, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<MachineWorkRecordDTO>>> BatchUpdateMachineWorkRecordsByUserIdAsync(string userId, IEnumerable<CreateOrModifyMachineWorkRecordDTO> workRecordDTOs, CancellationToken cancellationToken = default);

        Task<ServiceResult<MachineWorkRecordDTO>> ApproveMachineWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ServiceResult<MachineWorkRecordDTO>> RejectMachineWorkRecordByIdAsync(string id, string? rejectReason, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<MachineWorkRecordDTO>>> BatchApproveMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<MachineWorkRecordDTO>>> BatchRejectMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default);
    }
}