using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IEquipmentService
    {
        Task<ServiceResponse<IEnumerable<EquipmentDTO>>> GetEquipmentsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<EquipmentDTO>> GetEquipmentByIdAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<string>> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> UpdateEquipmentAsync(string equipmentId, UpdateEquipmentDTO updateEquipmentDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<EquipmentDTO>>> GetActiveEquipmentsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> EnableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DisableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);
    }
}