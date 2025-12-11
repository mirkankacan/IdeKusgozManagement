using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.MachineWorkRecordModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IMachineWorkRecordApiService
    {
        Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetMyMachineWorkRecordsByDateAsync(DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetMachineWorkRecordsByUserIdDateStatusAsync(string userId, DateTime date, int status, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetApprovedMachineWorkRecordsByUserAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchCreateOrModifyMachineWorkRecordsAsync(IEnumerable<CreateOrModifyMachineWorkRecordViewModel> createMachineWorkRecordViewModels, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchUpdateMachineWorkRecordsByUserIdAsync(string userId, IEnumerable<CreateOrModifyMachineWorkRecordViewModel> updateMachineWorkRecordViewModel, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchApproveMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchRejectMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default);

        Task<ApiResponse<MachineWorkRecordViewModel>> ApproveMachineWorkRecordByIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<MachineWorkRecordViewModel>> RejectMachineWorkRecordByIdAsync(string userId, string? rejectReason, CancellationToken cancellationToken = default);
    }
}