using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class EquipmentService(IUnitOfWork unitOfWork, ILogger<EquipmentService> logger) : IEquipmentService
    {
        public async Task<ServiceResult<IEnumerable<EquipmentDTO>>> GetEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var equipments = await unitOfWork.GetRepository<IdtEquipment>().GetAllAsync(cancellationToken);

                var equipmentDTOs = equipments.Adapt<IEnumerable<EquipmentDTO>>().OrderBy(x => x.GroupName).ThenByDescending(e => e.CreatedDate);

                return ServiceResult<IEnumerable<EquipmentDTO>>.SuccessAsOk(equipmentDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetEquipmentsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<EquipmentDTO>> GetEquipmentByIdAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await unitOfWork.GetRepository<IdtEquipment>().GetFirstOrDefaultAsync(e => e.Id == equipmentId, cancellationToken);

                if (equipment == null)
                {
                    return ServiceResult<EquipmentDTO>.Error("Ekipman Bulunamadı", "Belirtilen ID'ye sahip ekipman bulunamadı.", HttpStatusCode.NotFound);
                }

                var equipmentDTO = equipment.Adapt<EquipmentDTO>();

                return ServiceResult<EquipmentDTO>.SuccessAsOk(equipmentDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetEquipmentByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<string>> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingEquipment = await unitOfWork.GetRepository<IdtEquipment>().AnyAsync(e => e.Name.ToLower() == createEquipmentDTO.Name.ToLower(), cancellationToken);

                if (existingEquipment)
                {
                    return ServiceResult<string>.Error("Ekipman Zaten Mevcut", "Bu isimde bir ekipman zaten mevcut. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                var equipment = createEquipmentDTO.Adapt<IdtEquipment>();
                equipment.IsActive = true;
                await unitOfWork.GetRepository<IdtEquipment>().AddAsync(equipment, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResult<string>.SuccessAsCreated(equipment.Id, $"/api/equipments/{equipment.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateEquipmentAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> UpdateEquipmentAsync(string equipmentId, UpdateEquipmentDTO updateEquipmentDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await unitOfWork.GetRepository<IdtEquipment>().GetByIdAsync(equipmentId, cancellationToken);

                if (equipment == null)
                {
                    return ServiceResult<bool>.Error("Ekipman Bulunamadı", "Belirtilen ID'ye sahip ekipman bulunamadı.", HttpStatusCode.NotFound);
                }

                var existingEquipment = await unitOfWork.GetRepository<IdtEquipment>().AnyAsync(e => e.Name.ToLower() == updateEquipmentDTO.Name.ToLower() && e.Id != equipmentId, cancellationToken);

                if (existingEquipment)
                {
                    return ServiceResult<bool>.Error("Ekipman Zaten Mevcut", "Bu isimde başka bir ekipman zaten mevcut. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                updateEquipmentDTO.Adapt(equipment);

                unitOfWork.GetRepository<IdtEquipment>().Update(equipment);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateEquipmentAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await unitOfWork.GetRepository<IdtEquipment>().GetByIdAsync(equipmentId, cancellationToken);

                if (equipment == null)
                {
                    return ServiceResult<bool>.Error("Ekipman Bulunamadı", "Belirtilen ID'ye sahip ekipman bulunamadı.", HttpStatusCode.NotFound);
                }

                var isEquipmentUsed = await unitOfWork.GetRepository<IdtWorkRecord>().AnyAsync(wr => wr.EquipmentId == equipment.Id, cancellationToken) || await unitOfWork.GetRepository<IdtProject>().AnyAsync(x => equipmentId.Contains(x.TargetEquipmentIds), cancellationToken);

                if (isEquipmentUsed)
                {
                    return ServiceResult<bool>.Error("Silme İşlemi Başarısız", "Bu ekipman puantaj kayıtlarında kullanıldığı için silinemez.", HttpStatusCode.BadRequest);
                }

                unitOfWork.GetRepository<IdtEquipment>().Remove(equipment);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteEquipmentAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<EquipmentDTO>>> GetActiveEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var equipments = await unitOfWork.GetRepository<IdtEquipment>().Where(e => e.IsActive == true).OrderBy(e => e.GroupName).ThenByDescending(e => e.CreatedDate).ToListAsync(cancellationToken);

                var equipmentDTOs = equipments.Adapt<IEnumerable<EquipmentDTO>>();

                return ServiceResult<IEnumerable<EquipmentDTO>>.SuccessAsOk(equipmentDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveEquipmentsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DisableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await unitOfWork.GetRepository<IdtEquipment>().GetByIdAsync(equipmentId, cancellationToken);

                if (equipment == null)
                {
                    return ServiceResult<bool>.Error("Ekipman Bulunamadı", "Belirtilen ID'ye sahip ekipman bulunamadı.", HttpStatusCode.NotFound);
                }

                equipment.IsActive = false;

                unitOfWork.GetRepository<IdtEquipment>().Update(equipment);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableEquipmentAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> EnableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var equipment = await unitOfWork.GetRepository<IdtEquipment>().GetByIdAsync(equipmentId, cancellationToken);

                if (equipment == null)
                {
                    return ServiceResult<bool>.Error("Ekipman Bulunamadı", "Belirtilen ID'ye sahip ekipman bulunamadı.", HttpStatusCode.NotFound);
                }

                equipment.IsActive = true;

                unitOfWork.GetRepository<IdtEquipment>().Update(equipment);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableEquipmentAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}