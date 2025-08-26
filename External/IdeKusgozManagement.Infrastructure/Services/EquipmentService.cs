using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mapster;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EquipmentService> _logger;

        public EquipmentService(IUnitOfWork unitOfWork, ILogger<EquipmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<EquipmentListDTO>>> GetAllEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var equipments = await _unitOfWork.Repository<IdtEquipment>()
                    .GetAllAsync(cancellationToken);

                var equipmentDTOs = equipments
                    .Adapt<IEnumerable<EquipmentListDTO>>()
                    .OrderBy(e => e.Name);

                return ApiResponse<IEnumerable<EquipmentListDTO>>.Success(equipmentDTOs, "Ekipman listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ekipman listesi getirilirken hata oluştu");
                return ApiResponse<IEnumerable<EquipmentListDTO>>.Error("Ekipman listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<EquipmentDetailDTO>> GetEquipmentByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await _unitOfWork.Repository<IdtEquipment>()
                    .GetByIdAsync(id, cancellationToken);

                if (equipment == null)
                {
                    return ApiResponse<EquipmentDetailDTO>.Error("Ekipman bulunamadı");
                }

                var equipmentDTO = equipment.Adapt<EquipmentDetailDTO>();

                return ApiResponse<EquipmentDetailDTO>.Success(equipmentDTO, "Ekipman başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ekipman getirilirken hata oluştu. Id: {Id}", id);
                return ApiResponse<EquipmentDetailDTO>.Error("Ekipman getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if equipment with same name already exists
                var existingEquipment = await _unitOfWork.Repository<IdtEquipment>()
                    .FirstOrDefaultAsync(e => e.Name.ToLower() == createEquipmentDTO.Name.ToLower(), cancellationToken);

                if (existingEquipment != null)
                {
                    return ApiResponse<string>.Error("Bu isimde bir ekipman zaten mevcut");
                }

                var equipment = createEquipmentDTO.Adapt<IdtEquipment>();
                equipment.Name = equipment.Name.Trim();

                await _unitOfWork.Repository<IdtEquipment>().AddAsync(equipment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<string>.Success(equipment.Id, "Ekipman başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ekipman oluşturulurken hata oluştu. Name: {Name}", createEquipmentDTO.Name);
                return ApiResponse<string>.Error("Ekipman oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> UpdateEquipmentAsync(string id, UpdateEquipmentDTO updateEquipmentDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await _unitOfWork.Repository<IdtEquipment>()
                    .GetByIdAsync(id, cancellationToken);

                if (equipment == null)
                {
                    return ApiResponse<bool>.Error("Ekipman bulunamadı");
                }

                // Check if another equipment with same name already exists
                var existingEquipment = await _unitOfWork.Repository<IdtEquipment>()
                    .FirstOrDefaultAsync(e => e.Name.ToLower() == updateEquipmentDTO.Name.ToLower() && e.Id != id, cancellationToken);

                if (existingEquipment != null)
                {
                    return ApiResponse<bool>.Error("Bu isimde başka bir ekipman zaten mevcut");
                }

                updateEquipmentDTO.Adapt(equipment);
                equipment.Name = equipment.Name.Trim();

                await _unitOfWork.Repository<IdtEquipment>().UpdateAsync(equipment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Ekipman başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ekipman güncellenirken hata oluştu. Id: {Id}", id);
                return ApiResponse<bool>.Error("Ekipman güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteEquipmentAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await _unitOfWork.Repository<IdtEquipment>()
                    .GetByIdAsync(id, cancellationToken);

                if (equipment == null)
                {
                    return ApiResponse<bool>.Error("Ekipman bulunamadı");
                }

                // Check if equipment is used in any work records
                var isEquipmentUsed = await _unitOfWork.Repository<IdtWorkRecord>()
                    .AnyAsync(wr => wr.Equipment == equipment.Name, cancellationToken);

                if (isEquipmentUsed)
                {
                    return ApiResponse<bool>.Error("Bu ekipman iş kayıtlarında kullanıldığı için silinemez");
                }

                await _unitOfWork.Repository<IdtEquipment>().DeleteAsync(equipment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Ekipman başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ekipman silinirken hata oluştu. Id: {Id}", id);
                return ApiResponse<bool>.Error("Ekipman silinirken hata oluştu");
            }
        }
    }
}
