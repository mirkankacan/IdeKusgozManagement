using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.DepartmentModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IDepartmentApiService
    {
        Task<ApiResponse<IEnumerable<DepartmentViewModel>>> GetDepartmentsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>>> GetDepartmentDocumentTypeRelationsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DepartmentDocumentTypeViewModel>>> GetDepartmentDocumentTypeRelationsByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default);
    }
}