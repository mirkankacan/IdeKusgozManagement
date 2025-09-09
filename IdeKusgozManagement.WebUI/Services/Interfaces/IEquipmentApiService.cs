using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.EquipmentModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IEquipmentApiService
    {
        Task<ApiResponse<IEnumerable<EquipmentViewModel>>> GetEquipmentsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<EquipmentViewModel>>> GetActiveEquipmentsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<EquipmentViewModel>> GetEquipmentByIdAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> EnableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DisableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateEquipmentAsync(CreateEquipmentViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateEquipmentAsync(string equipmentId, UpdateEquipmentViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);
    }
}