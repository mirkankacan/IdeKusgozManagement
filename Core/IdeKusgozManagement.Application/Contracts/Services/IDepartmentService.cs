using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.DepartmentDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IDepartmentService
    {
        Task<ApiResponse<IEnumerable<DepartmentDTO>>> GetDepartmentsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DepartmentDocumentTypeDTO>>> GetDepartmentDocumentTypeRelationsAsync(CancellationToken cancellationToken = default);

        // Koşullular

        Task<ApiResponse<IEnumerable<DepartmentDocumentTypeDTO>>> GetDepartmentDocumentTypeRelationsByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default);
    }
}