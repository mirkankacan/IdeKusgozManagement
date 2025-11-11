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
        public async Task<ApiResponse<List<UserRequiredDocumentDTO>>> GetRequiredDocumentsAsync(string departmentId, string? targetId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(departmentId))
                    return ApiResponse<List<UserRequiredDocumentDTO>>.Error("Departman ID'si boş geçilemez");

                var parameters = new object[] { departmentId, targetId };
                var funcResults = await unitOfWork.ExecuteTableValuedFunctionAsync<UserRequiredDocumentDTO>(
                        "dbo.IDF_GetUserRequiredDocuments",
                        parameters,
                        cancellationToken);

                var resultList = funcResults.ToList();

                return resultList.Any()
                    ? ApiResponse<List<UserRequiredDocumentDTO>>.Success(resultList)
                    : ApiResponse<List<UserRequiredDocumentDTO>>.Error("Kullanıcının yüklemesi gereken evraklar bulunamadı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRequiredDocumentsAsync işleminde hata oluştu. DepartmentId: {DepartmentId}, TargetId: {TargetId}", departmentId, targetId);
                return ApiResponse<List<UserRequiredDocumentDTO>>.Error("Kullanıcının yüklemesi gereken evraklar listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var documents = await unitOfWork.GetRepository<IdtDocumentType>()
                    .WhereAsNoTracking(x => x.Id != null)
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (documents == null)
                {
                    return ApiResponse<IEnumerable<DocumentTypeDTO>>.Success(null, "Döküman tipleri bulunamadı");
                }

                var mappedDocuments = documents.Adapt<IEnumerable<DocumentTypeDTO>>();

                return ApiResponse<IEnumerable<DocumentTypeDTO>>.Success(mappedDocuments, "Döküman tipleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDocumentTypesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<DocumentTypeDTO>>.Error("Döküman tipleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var relatedIds = await unitOfWork.GetRepository<IdtDepartmentDocumentType>()
                    .WhereAsNoTracking(x => x.DepartmentId == departmentId)
                    .Select(x => x.DocumentTypeId)
                    .ToListAsync(cancellationToken);

                if (!relatedIds.Any())
                {
                    return ApiResponse<IEnumerable<DocumentTypeDTO>>.Success(null, "Departmanla ilgili döküman tipleri bulunamadı");
                }

                var documents = await unitOfWork.GetRepository<IdtDocumentType>()
                    .WhereAsNoTracking(x => relatedIds.Contains(x.Id))
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (documents == null)
                {
                    return ApiResponse<IEnumerable<DocumentTypeDTO>>.Success(null, "Departmanla ilgili döküman tipleri bulunamadı");
                }

                var mappedDocuments = documents.Adapt<IEnumerable<DocumentTypeDTO>>();

                return ApiResponse<IEnumerable<DocumentTypeDTO>>.Success(mappedDocuments, "Departmanla ilgili döküman tipleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDocumentTypesByDepartment işleminde hata oluştu");
                return ApiResponse<IEnumerable<DocumentTypeDTO>>.Error("Departmanla ilgili döküman tipleri getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<DocumentTypeDTO>> GetDocumentTypeByIdAsync(string documentTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var document = await unitOfWork.GetRepository<IdtDocumentType>().GetFirstOrDefaultAsync(x => x.Id == documentTypeId, cancellationToken);

                if (document == null)
                {
                    return ApiResponse<DocumentTypeDTO>.Error("Döküman tipi bulunamadı");
                }

                var mappedDocument = document.Adapt<DocumentTypeDTO>();

                return ApiResponse<DocumentTypeDTO>.Success(mappedDocument, "Döküman tipi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDocumentTypeByIdAsync işleminde hata oluştu");
                return ApiResponse<DocumentTypeDTO>.Error("Döküman tipi getirilirken hata oluştu");
            }
        }
    }
}