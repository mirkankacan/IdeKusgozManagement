using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.DocumentDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class DocumentService(IUnitOfWork unitOfWork, ILogger<DocumentService> logger, IIdentityService identityService) : IDocumentService
    {
        public async Task<ServiceResponse<IEnumerable<RequiredDocumentDTO>>> GetRequiredDocumentsAsync(string departmentId, string departmentDutyId, string? companyId, string? targetId, string? documentTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(departmentId))
                    return ServiceResponse<IEnumerable<RequiredDocumentDTO>>.Error("Departman ID'si boş geçilemez");
                if (string.IsNullOrEmpty(departmentDutyId))
                    return ServiceResponse<IEnumerable<RequiredDocumentDTO>>.Error("Departman görev ID'si boş geçilemez");

                var parameters = new object[] { departmentId, departmentDutyId, companyId, targetId, documentTypeId };
                var funcResults = await unitOfWork.ExecuteTableValuedFunctionAsync<RequiredDocumentDTO>(
                        "dbo.IDF_GetRequiredDocuments",
                        parameters,
                        cancellationToken);

                var resultList = funcResults
                    .OrderBy(x => x.CompanyName)
                    .ThenBy(x => x.DocumentTypeName)
                    .ToList();

                return ServiceResponse<IEnumerable<RequiredDocumentDTO>>.Success(resultList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRequiredDocumentsAsync işleminde hata oluştu. DepartmentId: {DepartmentId}, DepartmentDutyId: {DepartmentDutyId}, CompanyId: {CompanyId} TargetId: {TargetId}", departmentId, departmentDutyId, companyId, targetId);
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var documents = await unitOfWork.GetRepository<IdtDocumentType>()
                    .WhereAsNoTracking(x => x.Id != null)
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (documents == null)
                {
                    return ServiceResponse<IEnumerable<DocumentTypeDTO>>.Success(null, "Döküman tipleri bulunamadı");
                }

                var mappedDocuments = documents.Adapt<IEnumerable<DocumentTypeDTO>>();

                return ServiceResponse<IEnumerable<DocumentTypeDTO>>.Success(mappedDocuments, "Döküman tipleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDocumentTypesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<DocumentTypeDTO>> GetDocumentTypeByIdAsync(string documentTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var document = await unitOfWork.GetRepository<IdtDocumentType>().GetFirstOrDefaultAsync(x => x.Id == documentTypeId, cancellationToken);

                if (document == null)
                {
                    return ServiceResponse<DocumentTypeDTO>.Error("Döküman tipi bulunamadı");
                }

                var mappedDocument = document.Adapt<DocumentTypeDTO>();

                return ServiceResponse<DocumentTypeDTO>.Success(mappedDocument, "Döküman tipi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDocumentTypeByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesByDutyAsync(string departmentDutyId, /*string? companyId,*/ CancellationToken cancellationToken = default)
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
                    return ServiceResponse<IEnumerable<DocumentTypeDTO>>.Success(null, "Departman göreviyle ilgili döküman tipleri bulunamadı");
                }

                var documents = await unitOfWork.GetRepository<IdtDocumentType>()
                    .WhereAsNoTracking(x => relatedIds.Contains(x.Id))
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (documents == null)
                {
                    return ServiceResponse<IEnumerable<DocumentTypeDTO>>.Success(null, "Departman göreviyle ilgili döküman tipleri bulunamadı");
                }

                var mappedDocuments = documents.Adapt<IEnumerable<DocumentTypeDTO>>();

                return ServiceResponse<IEnumerable<DocumentTypeDTO>>.Success(mappedDocuments, "Departman göreviyle ilgili döküman tipleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDocumentTypesByDutyAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<string>> CreateDocumentTypeAsync(CreateDocumentTypeDTO createDocumentTypeDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingDocumentType = await unitOfWork.GetRepository<IdtDocumentType>().AnyAsync(dt => dt.Name.ToLower() == createDocumentTypeDTO.Name.ToLower(), cancellationToken);

                if (existingDocumentType)
                {
                    return ServiceResponse<string>.Error("Bu isimde bir doküman tipi zaten mevcut");
                }

                var documentType = createDocumentTypeDTO.Adapt<IdtDocumentType>();
                await unitOfWork.GetRepository<IdtDocumentType>().AddAsync(documentType, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResponse<string>.Success(documentType.Id, "Doküman tipi başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateDocumentTypeAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> UpdateDocumentTypeAsync(string documentTypeId, UpdateDocumentTypeDTO updateDocumentTypeDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var documentType = await unitOfWork.GetRepository<IdtDocumentType>().GetByIdAsync(documentTypeId, cancellationToken);

                if (documentType == null)
                {
                    return ServiceResponse<bool>.Error("Doküman tipi bulunamadı");
                }

                var existingDocumentType = await unitOfWork.GetRepository<IdtDocumentType>().AnyAsync(dt => dt.Name.ToLower() == updateDocumentTypeDTO.Name.ToLower() && dt.Id != documentTypeId, cancellationToken);

                if (existingDocumentType)
                {
                    return ServiceResponse<bool>.Error("Bu isimde başka bir doküman tipi zaten mevcut");
                }

                updateDocumentTypeDTO.Adapt(documentType);

                unitOfWork.GetRepository<IdtDocumentType>().Update(documentType);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Doküman tipi başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateDocumentTypeAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteDocumentTypeAsync(string documentTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var documentType = await unitOfWork.GetRepository<IdtDocumentType>().GetByIdAsync(documentTypeId, cancellationToken);

                if (documentType == null)
                {
                    return ServiceResponse<bool>.Error("Doküman tipi bulunamadı");
                }

                var isDocumentTypeUsed = await unitOfWork.GetRepository<IdtFile>().AnyAsync(f => f.DocumentTypeId == documentType.Id, cancellationToken);

                if (isDocumentTypeUsed)
                {
                    return ServiceResponse<bool>.Error("Bu doküman tipi dosya kayıtlarında kullanıldığı için silinemez");
                }

                unitOfWork.GetRepository<IdtDocumentType>().Remove(documentType);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Doküman tipi başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteDocumentTypeAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<string>> CreateDepartmentDocumentRequirmentAsync(CreateDepartmentDocumentRequirmentDTO createDTO, CancellationToken cancellationToken = default)
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
                    return ServiceResponse<string>.Error("Bu eşleştirme zaten mevcut");
                }

                var requirement = createDTO.Adapt<IdtDepartmentDocumentRequirment>();

                await unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>().AddAsync(requirement, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<string>.Success(requirement.Id, "Doküman tipi eşleştirmesi başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateDepartmentDocumentRequirmentAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<DepartmentDocumentRequirmentDTO>>> GetDepartmentDocumentRequirmentsAsync(CancellationToken cancellationToken = default)
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

                return ServiceResponse<IEnumerable<DepartmentDocumentRequirmentDTO>>.Success(mappedRequirements, "Eşleştirmeler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDepartmentDocumentRequirmentsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteDepartmentDocumentRequirmentAsync(string requirementId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(requirementId))
                {
                    return ServiceResponse<bool>.Error("Eşleştirme ID'si boş geçilemez");
                }

                var requirement = await unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>()
                    .GetByIdAsync(requirementId, cancellationToken);

                if (requirement == null)
                {
                    return ServiceResponse<bool>.Error("Eşleştirme bulunamadı");
                }

                unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>().Remove(requirement);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Eşleştirme başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteDepartmentDocumentRequirmentAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}