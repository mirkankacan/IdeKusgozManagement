using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.Extensions.Logging;

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

        public async Task<ApiResponse<IEnumerable<EquipmentDTO>>> GetAllEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var equipments = await _unitOfWork.Repository<IdtEquipment>().GetAllAsync(cancellationToken);

                var equipmentDTOs = equipments
                    .Adapt<IEnumerable<EquipmentDTO>>()
                    .OrderByDescending(e => e.CreatedDate);

                return ApiResponse<IEnumerable<EquipmentDTO>>.Success(equipmentDTOs, "Ekipman listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ekipman listesi getirilirken hata oluştu");
                return ApiResponse<IEnumerable<EquipmentDTO>>.Error("Ekipman listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<EquipmentDTO>> GetEquipmentByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await _unitOfWork.Repository<IdtEquipment>()
                    .GetByIdAsync(id, cancellationToken);

                if (equipment == null)
                {
                    return ApiResponse<EquipmentDTO>.Error("Ekipman bulunamadı");
                }

                var equipmentDTO = equipment.Adapt<EquipmentDTO>();

                return ApiResponse<EquipmentDTO>.Success(equipmentDTO, "Ekipman başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ekipman getirilirken hata oluştu. Id: {Id}", id);
                return ApiResponse<EquipmentDTO>.Error("Ekipman getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<string>> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO, CancellationToken cancellationToken = default)
        {
            try
            {
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

                var isEquipmentUsed = await _unitOfWork.Repository<IdtWorkRecord>()
                    .AnyAsync(wr => wr.EquipmentId == equipment.Id, cancellationToken);

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

        public async Task<ApiResponse<IEnumerable<EquipmentDTO>>> GetAllActiveEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var equipments = await _unitOfWork.Repository<IdtEquipment>().GetWhereAsync(x => x.IsActive == true, cancellationToken);

                var equipmentDTOs = equipments
                    .Adapt<IEnumerable<EquipmentDTO>>()
                    .OrderBy(e => e.Name);

                return ApiResponse<IEnumerable<EquipmentDTO>>.Success(equipmentDTOs, "Aktif ekipman listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif ekipman listesi getirilirken hata oluştu");
                return ApiResponse<IEnumerable<EquipmentDTO>>.Error("Aktif ekipman listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeactivateEquipmentAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtEquipment>()
                    .GetByIdAsync(id, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Ekipman bulunamadı");
                }

                expense.IsActive = false;

                await _unitOfWork.Repository<IdtEquipment>().UpdateAsync(expense, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Ekipman başarıyla pasif duruma getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ekipman pasif duruma getirilirken hata oluştu. Id: {Id}", id);
                return ApiResponse<bool>.Error("Ekipman pasif duruma getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> ActivateEquipmentAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var expense = await _unitOfWork.Repository<IdtEquipment>()
                    .GetByIdAsync(id, cancellationToken);

                if (expense == null)
                {
                    return ApiResponse<bool>.Error("Ekipman bulunamadı");
                }

                expense.IsActive = true;

                await _unitOfWork.Repository<IdtEquipment>().UpdateAsync(expense, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Ekipman başarıyla aktif duruma getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ekipman aktif duruma getirilirken hata oluştu. Id: {Id}", id);
                return ApiResponse<bool>.Error("Ekipman aktif duruma getirilirken hata oluştu");
            }
        }
    }
}