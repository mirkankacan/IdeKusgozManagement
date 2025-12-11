using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.DocumentModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IDocumentApiService
    {
        Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesByDutyAsync(string departmentDutyId, /*string? companyId, */CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<RequiredDocumentViewModel>>> GetRequiredDocumentsAsync(string departmentId, string departmentDutyId, string? companyId, string? targetId, string? documentTypeId, CancellationToken cancellationToken = default);
    }
}