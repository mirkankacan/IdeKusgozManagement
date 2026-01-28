using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.DocumentDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class DocumentService(IUnitOfWork unitOfWork, ILogger<DocumentService> logger, IIdentityService identityService) : IDocumentService
    {
        public async Task<ServiceResult<IEnumerable<RequiredDocumentDTO>>> GetRequiredDocumentsAsync(string departmentId, string departmentDutyId, string? companyId, string? targetId, string? documentTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(departmentId))
                    return ServiceResult<IEnumerable<RequiredDocumentDTO>>.Error("Validasyon Hatası", "Departman ID'si boş geçilemez.", HttpStatusCode.BadRequest);
                if (string.IsNullOrEmpty(departmentDutyId))
                    return ServiceResult<IEnumerable<RequiredDocumentDTO>>.Error("Validasyon Hatası", "Departman görev ID'si boş geçilemez.", HttpStatusCode.BadRequest);

                var parameters = new object[] { departmentId, departmentDutyId, companyId, targetId, documentTypeId };
                var funcResults = await unitOfWork.ExecuteTableValuedFunctionAsync<RequiredDocumentDTO>(
                        "dbo.IDF_GetRequiredDocuments",
                        parameters,
                        cancellationToken);

                var resultList = funcResults
                    .OrderBy(x => x.CompanyName)
                    .ThenBy(x => x.DocumentTypeName)
                    .ToList();

                return ServiceResult<IEnumerable<RequiredDocumentDTO>>.SuccessAsOk(resultList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRequiredDocumentsAsync işleminde hata oluştu. DepartmentId: {DepartmentId}, DepartmentDutyId: {DepartmentDutyId}, CompanyId: {CompanyId} TargetId: {TargetId}", departmentId, departmentDutyId, companyId, targetId);
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var documents = await unitOfWork.GetRepository<IdtDocumentType>()
                    .WhereAsNoTracking(x => x.Id != null)
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (documents == null || !documents.Any())
                {
                    return ServiceResult<IEnumerable<DocumentTypeDTO>>.SuccessAsOk(Enumerable.Empty<DocumentTypeDTO>());
                }

                var mappedDocuments = documents.Adapt<IEnumerable<DocumentTypeDTO>>();

                return ServiceResult<IEnumerable<DocumentTypeDTO>>.SuccessAsOk(mappedDocuments);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDocumentTypesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<DocumentTypeDTO>> GetDocumentTypeByIdAsync(string documentTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var document = await unitOfWork.GetRepository<IdtDocumentType>().GetFirstOrDefaultAsync(x => x.Id == documentTypeId, cancellationToken);

                if (document == null)
                {
                    return ServiceResult<DocumentTypeDTO>.Error("Döküman Tipi Bulunamadı", "Belirtilen ID'ye sahip döküman tipi bulunamadı.", HttpStatusCode.NotFound);
                }

                var mappedDocument = document.Adapt<DocumentTypeDTO>();

                return ServiceResult<DocumentTypeDTO>.SuccessAsOk(mappedDocument);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDocumentTypeByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesByDutyAsync(string departmentDutyId, /*string? companyId,*/ CancellationToken cancellationToken = default)
        {
            try
            {
                IQueryable<IdtDepartmentDocumentRequirment> relatedIdsQuery;
                relatedIdsQuery = unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>().WhereAsNoTracking(x => x.DepartmentDutyId == departmentDutyId);
                //if (!string.IsNullOrEmpty(companyId))
                //    relatedIdsQuery = unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>().WhereAsNoTracking(x => x.CompanyId == companyId);

                var relatedIds = await relatedIdsQuery
                .OrderBy(x => x.DocumentTypeId)
                .Select(x => x.DocumentTypeId)
                .ToListAsync(cancellationToken);

                if (!relatedIds.Any())
                {
                    return ServiceResult<IEnumerable<DocumentTypeDTO>>.SuccessAsOk(Enumerable.Empty<DocumentTypeDTO>());
                }

                var documents = await unitOfWork.GetRepository<IdtDocumentType>()
                    .WhereAsNoTracking(x => relatedIds.Contains(x.Id))
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (documents == null || !documents.Any())
                {
                    return ServiceResult<IEnumerable<DocumentTypeDTO>>.SuccessAsOk(Enumerable.Empty<DocumentTypeDTO>());
                }

                var mappedDocuments = documents.Adapt<IEnumerable<DocumentTypeDTO>>();

                return ServiceResult<IEnumerable<DocumentTypeDTO>>.SuccessAsOk(mappedDocuments);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDocumentTypesByDutyAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<string>> CreateDocumentTypeAsync(CreateDocumentTypeDTO createDocumentTypeDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingDocumentType = await unitOfWork.GetRepository<IdtDocumentType>().AnyAsync(dt => dt.Name.ToLower() == createDocumentTypeDTO.Name.ToLower(), cancellationToken);

                if (existingDocumentType)
                {
                    return ServiceResult<string>.Error("Döküman Tipi Zaten Mevcut", "Bu isimde bir doküman tipi zaten mevcut. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                var documentType = createDocumentTypeDTO.Adapt<IdtDocumentType>();
                await unitOfWork.GetRepository<IdtDocumentType>().AddAsync(documentType, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResult<string>.SuccessAsCreated(documentType.Id, $"/api/documents/types/{documentType.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateDocumentTypeAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> UpdateDocumentTypeAsync(string documentTypeId, UpdateDocumentTypeDTO updateDocumentTypeDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var documentType = await unitOfWork.GetRepository<IdtDocumentType>().GetByIdAsync(documentTypeId, cancellationToken);

                if (documentType == null)
                {
                    return ServiceResult<bool>.Error("Döküman Tipi Bulunamadı", "Belirtilen ID'ye sahip döküman tipi bulunamadı.", HttpStatusCode.NotFound);
                }

                var existingDocumentType = await unitOfWork.GetRepository<IdtDocumentType>().AnyAsync(dt => dt.Name.ToLower() == updateDocumentTypeDTO.Name.ToLower() && dt.Id != documentTypeId, cancellationToken);

                if (existingDocumentType)
                {
                    return ServiceResult<bool>.Error("Döküman Tipi Zaten Mevcut", "Bu isimde başka bir doküman tipi zaten mevcut. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                updateDocumentTypeDTO.Adapt(documentType);

                unitOfWork.GetRepository<IdtDocumentType>().Update(documentType);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateDocumentTypeAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteDocumentTypeAsync(string documentTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var documentType = await unitOfWork.GetRepository<IdtDocumentType>().GetByIdAsync(documentTypeId, cancellationToken);

                if (documentType == null)
                {
                    return ServiceResult<bool>.Error("Döküman Tipi Bulunamadı", "Belirtilen ID'ye sahip döküman tipi bulunamadı.", HttpStatusCode.NotFound);
                }

                var isDocumentTypeUsed = await unitOfWork.GetRepository<IdtFile>().AnyAsync(f => f.DocumentTypeId == documentType.Id, cancellationToken);

                if (isDocumentTypeUsed)
                {
                    return ServiceResult<bool>.Error("Silme İşlemi Başarısız", "Bu doküman tipi dosya kayıtlarında kullanıldığı için silinemez.", HttpStatusCode.BadRequest);
                }

                unitOfWork.GetRepository<IdtDocumentType>().Remove(documentType);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteDocumentTypeAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<string>> CreateDepartmentDocumentRequirmentAsync(CreateDepartmentDocumentRequirmentDTO createDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if the requirement already exists
                var existingRequirement = await unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>()
                    .AnyAsync(r => r.DepartmentId == createDTO.DepartmentId
                        && r.DepartmentDutyId == createDTO.DepartmentDutyId
                        && r.DocumentTypeId == createDTO.DocumentTypeId
                        && (createDTO.CompanyId == null ? r.CompanyId == null : r.CompanyId == createDTO.CompanyId),
                        cancellationToken);

                if (existingRequirement)
                {
                    return ServiceResult<string>.Error("Eşleştirme Zaten Mevcut", "Bu eşleştirme zaten mevcut.", HttpStatusCode.BadRequest);
                }

                var requirement = createDTO.Adapt<IdtDepartmentDocumentRequirment>();

                await unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>().AddAsync(requirement, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<string>.SuccessAsCreated(requirement.Id, $"/api/documents/requirements/{requirement.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateDepartmentDocumentRequirmentAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<DepartmentDocumentRequirmentDTO>>> GetDepartmentDocumentRequirmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var requirements = await unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>()
                    .WhereAsNoTracking(x => x.Id != null)
                    .Include(r => r.Department)
                    .Include(r => r.DepartmentDuty)
                    .Include(r => r.DocumentType)
                    .Include(r => r.Company)
                    .OrderBy(r => r.Department.Name)
                    .ThenBy(r => r.DepartmentDuty.Name)
                    .ThenBy(r => r.DocumentType.Name)
                    .ThenBy(r => r.Company.Name)
                    .ToListAsync(cancellationToken);

                var mappedRequirements = requirements.Adapt<IEnumerable<DepartmentDocumentRequirmentDTO>>();

                return ServiceResult<IEnumerable<DepartmentDocumentRequirmentDTO>>.SuccessAsOk(mappedRequirements);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDepartmentDocumentRequirmentsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteDepartmentDocumentRequirmentAsync(string requirementId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(requirementId))
                {
                    return ServiceResult<bool>.Error("Validasyon Hatası", "Eşleştirme ID'si boş geçilemez.", HttpStatusCode.BadRequest);
                }

                var requirement = await unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>()
                    .GetByIdAsync(requirementId, cancellationToken);

                if (requirement == null)
                {
                    return ServiceResult<bool>.Error("Eşleştirme Bulunamadı", "Belirtilen ID'ye sahip eşleştirme bulunamadı.", HttpStatusCode.NotFound);
                }

                unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>().Remove(requirement);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteDepartmentDocumentRequirmentAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}