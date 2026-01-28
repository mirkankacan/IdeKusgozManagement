using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.ParameterDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class ParameterService(IUnitOfWork unitOfWork, ILogger<ParameterService> logger) : IParameterService
    {
        public async Task<ServiceResult<IEnumerable<ParameterDTO>>> GetParametersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var parameters = await unitOfWork.GetRepository<IdtParameter>()
                    .GetAllAsync(cancellationToken);

                if (!parameters.Any())
                {
                    return ServiceResult<IEnumerable<ParameterDTO>>.Error("Parametreler Bulunamadı", $"Veri tabanında herhangi bir parametre bulunamadı.", HttpStatusCode.NotFound);
                }
                var dto = parameters.Select(p => new ParameterDTO
                {
                    Id = p.Id,
                    Key = p.Key,
                    Value = p.Value,
                    CreatedBy = p.CreatedBy,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate
                });
                return ServiceResult<IEnumerable<ParameterDTO>>.SuccessAsOk(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetParametersAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<ParameterDTO>> GetParameterByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var parameter = await unitOfWork.GetRepository<IdtParameter>()
                    .WhereAsNoTracking(p => p.Key == key)
                    .FirstOrDefaultAsync(cancellationToken);

                if (parameter == null)
                {
                    return ServiceResult<ParameterDTO>.Error("Parametre Bulunamadı", $"'{key}' anahtarına sahip parametre bulunamadı.", HttpStatusCode.NotFound);
                }
                var dto = new ParameterDTO()
                {
                    Id = parameter.Id,
                    Key = parameter.Key,
                    Value = parameter.Value,
                    CreatedBy = parameter.CreatedBy,
                    CreatedDate = parameter.CreatedDate,
                    UpdatedDate = parameter.UpdatedDate
                };
                return ServiceResult<ParameterDTO>.SuccessAsOk(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetParameterByKeyAsync işleminde hata oluştu. Key: {Key}", key);
                throw;
            }
        }

        public async Task<ServiceResult<string>> CreateParameterAsync(CreateParameterDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if parameter already exists
                var existingParameter = await unitOfWork.GetRepository<IdtParameter>()
                    .WhereAsNoTracking(p => p.Key == dto.Key)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingParameter != null)
                {
                    return ServiceResult<string>.Error("Parametre Zaten Mevcut", $"'{dto.Key}' anahtarına sahip bir parametre zaten mevcut", HttpStatusCode.Conflict);
                }

                var parameter = new IdtParameter
                {
                    Key = dto.Key,
                    Value = dto.Value
                };

                await unitOfWork.GetRepository<IdtParameter>().AddAsync(parameter, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<string>.SuccessAsCreated(parameter.Id, $"/api/parameters/{dto.Key}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateParameterAsync işleminde hata oluştu. Key: {Key}, Value: {Value}", dto.Key, dto.Value);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> UpdateParameterAsync(string id, UpdateParameterDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id != dto.Id)
                    return ServiceResult<bool>.Error("ID'ler Eşleşmiyor", "Güncellenmek istenen parametre ID'leri eşleşmiyor", HttpStatusCode.BadRequest);

                var parameter = await unitOfWork.GetRepository<IdtParameter>()
                    .Where(p => p.Id == dto.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (parameter == null)
                {
                    return ServiceResult<bool>.Error("Parametre Bulunamadı", $"'{dto.Id}' ID'sine sahip parametre bulunamadı", HttpStatusCode.NotFound);
                }

                parameter.Value = dto.Value;

                unitOfWork.GetRepository<IdtParameter>().Update(parameter);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateParameterAsync işleminde hata oluştu. Value: {Value}", dto.Value);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteParameterAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var parameter = await unitOfWork.GetRepository<IdtParameter>()
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (parameter == null)
                {
                    return ServiceResult<bool>.Error("Parametre Bulunamadı", $"'{id}' ID'sine sahip parametre bulunamadı.", HttpStatusCode.NotFound);
                }

                unitOfWork.GetRepository<IdtParameter>().Remove(parameter);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteParameterAsync işleminde hata oluştu. Id: {Id}", id);
                throw;
            }
        }
    }
}