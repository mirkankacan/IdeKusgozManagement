using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IEquipmentService
    {
        Task<ServiceResult<IEnumerable<EquipmentDTO>>> GetEquipmentsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<EquipmentDTO>> GetEquipmentByIdAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ServiceResult<string>> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> UpdateEquipmentAsync(string equipmentId, UpdateEquipmentDTO updateEquipmentDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<EquipmentDTO>>> GetActiveEquipmentsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> EnableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DisableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default);
    }
}