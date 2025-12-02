using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.DepartmentDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class DepartmentService(IUnitOfWork unitOfWork, ILogger<DepartmentService> logger) : IDepartmentService
    {
        //public async Task<ServiceResponse<IEnumerable<DepartmentDutyDocumentRelationDTO>>> GetDepartmentDutyDocumentRelationsAsync(string? departmentId, string? departmentDutyId, string? companyId, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        IQueryable<IdtDepartmentDocumentRequirment> baseQuery;
        //        baseQuery = unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>().WhereAsNoTracking(x => x.Id != null);
        //        if (!string.IsNullOrEmpty(departmentId))
        //            baseQuery = unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>().WhereAsNoTracking(x => x.DepartmentId != departmentId);

        //        if (!string.IsNullOrEmpty(departmentDutyId))
        //            baseQuery = unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>().WhereAsNoTracking(x => x.DepartmentDutyId != departmentDutyId);

        //        if (!string.IsNullOrEmpty(companyId))
        //            baseQuery = unitOfWork.GetRepository<IdtDepartmentDocumentRequirment>().WhereAsNoTracking(x => x.CompanyId != companyId);

        //        var reqs = await baseQuery
        //           .OrderBy(x => x.CompanyId)
        //           .ToListAsync(cancellationToken);

        //        if (reqs == null)
        //        {
        //            return ServiceResponse<IEnumerable<DepartmentDutyDocumentRelationDTO>>.Success(null, "Departman belge gereklilikleri bulunamadı");
        //        }

        //        var mappedReqs = reqs.Adapt<IEnumerable<DepartmentDutyDocumentRelationDTO>>();

        //        return ServiceResponse<IEnumerable<DepartmentDutyDocumentRelationDTO>>.Success(mappedReqs, "Departman belge gereklilikleri başarıyla getirildi");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "GetDepartmentDutyDocumentRelationsAsync işleminde hata oluştu");
        //        return ServiceResponse<IEnumerable<DepartmentDutyDocumentRelationDTO>>.Error("Departman belge gereklilikleri getirilirken hata oluştu");
        //    }
        //}

        public async Task<ServiceResponse<IEnumerable<DepartmentDutyDTO>>> GetDepartmentDutiesByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var duties = await unitOfWork.GetRepository<IdtDepartmentDuty>()
                    .WhereAsNoTracking(x => x.DepartmentId == departmentId)
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (duties == null)
                {
                    return ServiceResponse<IEnumerable<DepartmentDutyDTO>>.Success(null, "Departman görevleri bulunamadı");
                }

                var mappedDuties = duties.Adapt<IEnumerable<DepartmentDutyDTO>>();

                return ServiceResponse<IEnumerable<DepartmentDutyDTO>>.Success(mappedDuties, "Departman görevleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDepartmentDutiesByDepartment işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<DepartmentDTO>>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var departments = await unitOfWork.GetRepository<IdtDepartment>()
                    .WhereAsNoTracking(x => x.Id != null)
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (departments == null)
                {
                    return ServiceResponse<IEnumerable<DepartmentDTO>>.Success(null, "Departmanlar bulunamadı");
                }

                var mappedDepartments = departments.Adapt<IEnumerable<DepartmentDTO>>();

                return ServiceResponse<IEnumerable<DepartmentDTO>>.Success(mappedDepartments, "Departmanlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetDepartmentsAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}