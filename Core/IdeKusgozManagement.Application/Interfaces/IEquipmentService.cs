using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IEquipmentService
    {
        Task<ApiResponse<IEnumerable<EquipmentListDTO>>> GetAllEquipmentsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<EquipmentDetailDTO>> GetEquipmentByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<ApiResponse<string>> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> UpdateEquipmentAsync(string id, UpdateEquipmentDTO updateEquipmentDTO, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteEquipmentAsync(string id, CancellationToken cancellationToken = default);
    }
}
