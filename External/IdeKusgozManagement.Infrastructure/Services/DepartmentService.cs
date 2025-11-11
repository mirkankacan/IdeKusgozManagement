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
    }
}