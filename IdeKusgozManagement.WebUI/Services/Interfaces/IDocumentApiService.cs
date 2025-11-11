using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.DocumentModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IDocumentApiService
    {
        Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<List<UserRequiredDocumentViewModel>>> GetRequiredDocumentsAsync(string departmentId, string? targetId, CancellationToken cancellationToken = default);
    }
}