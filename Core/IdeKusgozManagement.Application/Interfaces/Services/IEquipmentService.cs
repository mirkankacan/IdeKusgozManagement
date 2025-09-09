using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IEquipmentService
    {
        Task<ApiResponse<IEnumerable<EquipmentDTO>>> GetEquipmentsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<EquipmentDTO>>> GetActiveEquipmentsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<EquipmentDTO>> GetEquipmentByIdAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> EnableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DisableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateEquipmentAsync(string equipmentId, UpdateEquipmentDTO updateEquipmentDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);
    }
}