using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class EquipmentService(IUnitOfWork unitOfWork, ILogger<EquipmentService> logger) : IEquipmentService
    {
        public async Task<ServiceResponse<IEnumerable<EquipmentDTO>>> GetEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var equipments = (await unitOfWork.GetRepository<IdtEquipment>().GetAllAsync(cancellationToken)).OrderByDescending(e => e.CreatedDate);

                var equipmentDTOs = equipments.Adapt<IEnumerable<EquipmentDTO>>();

                return ServiceResponse<IEnumerable<EquipmentDTO>>.Success(equipmentDTOs, "Ekipman listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetEquipmentsAsync işleminde hata oluştu");
                return ServiceResponse<IEnumerable<EquipmentDTO>>.Error("Ekipman listesi getirilirken hata oluştu");
            }
        }

        public async Task<ServiceResponse<EquipmentDTO>> GetEquipmentByIdAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await unitOfWork.GetRepository<IdtEquipment>().GetFirstOrDefaultAsync(e => e.Id == equipmentId, cancellationToken);

                if (equipment == null)
                {
                    return ServiceResponse<EquipmentDTO>.Error("Ekipman bulunamadı");
                }

                var equipmentDTO = equipment.Adapt<EquipmentDTO>();

                return ServiceResponse<EquipmentDTO>.Success(equipmentDTO, "Ekipman başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetEquipmentByIdAsync işleminde hata oluştu");
                return ServiceResponse<EquipmentDTO>.Error("Ekipman getirilirken hata oluştu");
            }
        }

        public async Task<ServiceResponse<string>> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingEquipment = await unitOfWork.GetRepository<IdtEquipment>().AnyAsync(e => e.Name.ToLower() == createEquipmentDTO.Name.ToLower(), cancellationToken);

                if (existingEquipment)
                {
                    return ServiceResponse<string>.Error("Bu isimde bir ekipman zaten mevcut");
                }

                var equipment = createEquipmentDTO.Adapt<IdtEquipment>();
                equipment.IsActive = true;
                await unitOfWork.GetRepository<IdtEquipment>().AddAsync(equipment, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResponse<string>.Success(equipment.Id, "Ekipman başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateEquipmentAsync işleminde hata oluştu");
                return ServiceResponse<string>.Error("Ekipman oluşturulurken hata oluştu");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateEquipmentAsync(string equipmentId, UpdateEquipmentDTO updateEquipmentDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await unitOfWork.GetRepository<IdtEquipment>().GetByIdAsync(equipmentId, cancellationToken);

                if (equipment == null)
                {
                    return ServiceResponse<bool>.Error("Ekipman bulunamadı");
                }

                var existingEquipment = await unitOfWork.GetRepository<IdtEquipment>().AnyAsync(e => e.Name.ToLower() == updateEquipmentDTO.Name.ToLower() && e.Id != equipmentId, cancellationToken);

                if (existingEquipment)
                {
                    return ServiceResponse<bool>.Error("Bu isimde başka bir ekipman zaten mevcut");
                }

                updateEquipmentDTO.Adapt(equipment);

                unitOfWork.GetRepository<IdtEquipment>().Update(equipment);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Ekipman başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateEquipmentAsync işleminde hata oluştu");
                return ServiceResponse<bool>.Error("Ekipman güncellenirken hata oluştu");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await unitOfWork.GetRepository<IdtEquipment>().GetByIdAsync(equipmentId, cancellationToken);

                if (equipment == null)
                {
                    return ServiceResponse<bool>.Error("Ekipman bulunamadı");
                }

                var isEquipmentUsed = await unitOfWork.GetRepository<IdtWorkRecord>().AnyAsync(wr => wr.EquipmentId == equipment.Id, cancellationToken);

                if (isEquipmentUsed)
                {
                    return ServiceResponse<bool>.Error("Bu ekipman puantaj kayıtlarında kullanıldığı için silinemez");
                }

                unitOfWork.GetRepository<IdtEquipment>().Remove(equipment);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Ekipman başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteEquipmentAsync işleminde hata oluştu");
                return ServiceResponse<bool>.Error("Ekipman silinirken hata oluştu");
            }
        }

        public async Task<ServiceResponse<IEnumerable<EquipmentDTO>>> GetActiveEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var equipments = await unitOfWork.GetRepository<IdtEquipment>().Where(e => e.IsActive == true).OrderBy(e => e.Name).ToListAsync(cancellationToken);

                var equipmentDTOs = equipments.Adapt<IEnumerable<EquipmentDTO>>();

                return ServiceResponse<IEnumerable<EquipmentDTO>>.Success(equipmentDTOs, "Aktif ekipman listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveEquipmentsAsync işleminde hata oluştu");
                return ServiceResponse<IEnumerable<EquipmentDTO>>.Error("Aktif ekipman listesi getirilirken hata oluştu");
            }
        }

        public async Task<ServiceResponse<bool>> DisableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await unitOfWork.GetRepository<IdtEquipment>().GetByIdAsync(equipmentId, cancellationToken);

                if (equipment == null)
                {
                    return ServiceResponse<bool>.Error("Ekipman bulunamadı");
                }

                equipment.IsActive = false;

                unitOfWork.GetRepository<IdtEquipment>().Update(equipment);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Ekipman başarıyla pasif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableEquipmentAsync işleminde hata oluştu");
                return ServiceResponse<bool>.Error("Ekipman pasif duruma getirilirken hata oluştu");
            }
        }

        public async Task<ServiceResponse<bool>> EnableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await unitOfWork.GetRepository<IdtEquipment>().GetByIdAsync(equipmentId, cancellationToken);

                if (equipment == null)
                {
                    return ServiceResponse<bool>.Error("Ekipman bulunamadı");
                }

                equipment.IsActive = true;

                unitOfWork.GetRepository<IdtEquipment>().Update(equipment);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Ekipman başarıyla aktif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableEquipmentAsync işleminde hata oluştu");
                return ServiceResponse<bool>.Error("Ekipman aktif duruma getirilirken hata oluştu");
            }
        }
    }
}