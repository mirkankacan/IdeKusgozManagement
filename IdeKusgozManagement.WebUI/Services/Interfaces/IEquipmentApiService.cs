using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.EquipmentModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IEquipmentApiService
    {
        Task<ApiResponse<IEnumerable<EquipmentViewModel>>> GetAllEquipmentsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<EquipmentViewModel>> GetEquipmentByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<ApiResponse<string>> CreateEquipmentAsync(CreateEquipmentViewModel model, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> UpdateEquipmentAsync(string id, UpdateEquipmentViewModel model, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteEquipmentAsync(string id, CancellationToken cancellationToken = default);
    }
}
