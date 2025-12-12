using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.DocumentModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IDocumentApiService
    {
        Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesByDutyAsync(string departmentDutyId, /*string? companyId, */CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<RequiredDocumentViewModel>>> GetRequiredDocumentsAsync(string departmentId, string departmentDutyId, string? companyId, string? targetId, string? documentTypeId, CancellationToken cancellationToken = default);

        Task<ApiResponse<DocumentTypeViewModel>> GetDocumentTypeByIdAsync(string documentTypeId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateDocumentTypeAsync(CreateDocumentTypeViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateDocumentTypeAsync(string documentTypeId, UpdateDocumentTypeViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteDocumentTypeAsync(string documentTypeId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateDepartmentDocumentRequirmentAsync(CreateDepartmentDocumentRequirmentViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DepartmentDocumentRequirmentViewModel>>> GetDepartmentDocumentRequirmentsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteDepartmentDocumentRequirmentAsync(string requirementId, CancellationToken cancellationToken = default);
    }
}