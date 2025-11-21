using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.DepartmentDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IDepartmentService
    {
        Task<ServiceResponse<IEnumerable<DepartmentDTO>>> GetDepartmentsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<DepartmentDutyDTO>>> GetDepartmentDutiesByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default);

        //Task<ServiceResponse<IEnumerable<DepartmentDutyDocumentRelationDTO>>> GetDepartmentDutyDocumentRelationsAsync(string? departmentId, string? departmentDutyId, string? companyId, CancellationToken cancellationToken = default);
    }
}