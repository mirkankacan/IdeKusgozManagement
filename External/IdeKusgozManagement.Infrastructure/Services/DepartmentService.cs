using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.DepartmentDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class DepartmentService(IUnitOfWork unitOfWork, ILogger<DepartmentService> logger, UserManager<ApplicationUser> userManager) : IDepartmentService
    {
        public async Task<ApiResponse<IEnumerable<DepartmentDTO>>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var departments = await unitOfWork.GetRepository<IdtDepartment>()
                    .WhereAsNoTracking(x => x.Id != null)
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (departments == null)
                {
                    return ApiResponse<IEnumerable<DepartmentDTO>>.Success(null, "Departmanlar bulunamadı");
                }

                var mappedDepartments = departments.Adapt<IEnumerable<DepartmentDTO>>();

                return ApiResponse<IEnumerable<DepartmentDTO>>.Success(mappedDepartments, "Departmanlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDepartmentsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<DepartmentDTO>>.Error("Departmanlar getirilirken hata oluştu");
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

        public async Task<ApiResponse<IEnumerable<DepartmentDocumentTypeDTO>>> GetDepartmentDocumentTypeRelationsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var ddt = await unitOfWork.GetRepository<IdtDepartmentDocumentType>()
                    .WhereAsNoTracking(x => x.Id != null)
                    .OrderBy(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (ddt == null)
                {
                    return ApiResponse<IEnumerable<DepartmentDocumentTypeDTO>>.Success(null, "Departman döküman bağlantıları bulunamadı");
                }

                var mappedDdt = ddt.Adapt<IEnumerable<DepartmentDocumentTypeDTO>>();

                return ApiResponse<IEnumerable<DepartmentDocumentTypeDTO>>.Success(mappedDdt, "Departman döküman bağlantıları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDepartmentDocumentTypesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<DepartmentDocumentTypeDTO>>.Error("Departman döküman bağlantıları getirilirken hata oluştu");
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

        public async Task<ApiResponse<IEnumerable<DepartmentDocumentTypeDTO>>> GetDepartmentDocumentTypeRelationsByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var ddt = await unitOfWork.GetRepository<IdtDepartmentDocumentType>()
                    .WhereAsNoTracking(x => x.DepartmentId == departmentId)
                    .OrderBy(x => x.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (ddt == null)
                {
                    return ApiResponse<IEnumerable<DepartmentDocumentTypeDTO>>.Success(null, "Departman döküman bağlantıları bulunamadı");
                }

                var mappedDdt = ddt.Adapt<IEnumerable<DepartmentDocumentTypeDTO>>();

                return ApiResponse<IEnumerable<DepartmentDocumentTypeDTO>>.Success(mappedDdt, "Departman döküman bağlantıları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDepartmentDocumentTypeRelationsByDepartmentAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<DepartmentDocumentTypeDTO>>.Error("Departman döküman bağlantıları getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<DocumentTypeDTO>> GetDocumentTypeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {

                var documents = await unitOfWork.GetRepository<IdtDocumentType>()
                    .GetFirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (documents == null)
                {
                    return ApiResponse<DocumentTypeDTO>.Success(null, "Döküman tipi bulunamadı");
                }

                var mappedDocuments = documents.Adapt<DocumentTypeDTO>();

                return ApiResponse<DocumentTypeDTO>.Success(mappedDocuments, "Döküman tipi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDocumentTypeByIdAsync işleminde hata oluştu");
                return ApiResponse<DocumentTypeDTO>.Error("Döküman tipi getirilirken hata oluştu");
            }
        }
    }
}