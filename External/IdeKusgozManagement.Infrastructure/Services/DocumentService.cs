using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.DocumentDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class DocumentService(IUnitOfWork unitOfWork, ILogger<DocumentService> logger) : IDocumentService
    {
        public async Task<ServiceResponse<IEnumerable<RequiredDocumentDTO>>> GetRequiredDocumentsAsync(string departmentId, string departmentDutyId, string? companyId, string? targetId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(departmentId))
                    return ServiceResponse<IEnumerable<RequiredDocumentDTO>>.Error("Departman ID'si boş geçilemez");
                if (string.IsNullOrEmpty(departmentDutyId))
                    return ServiceResponse<IEnumerable<RequiredDocumentDTO>>.Error("Departman görev ID'si boş geçilemez");

                var parameters = new object[] { departmentId, departmentDutyId, companyId, targetId };
                var funcResults = await unitOfWork.ExecuteTableValuedFunctionAsync<RequiredDocumentDTO>(
                        "dbo.IDF_GetRequiredDocuments",
                        parameters,
                        cancellationToken);

                var resultList = funcResults.ToList();

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
    }
}