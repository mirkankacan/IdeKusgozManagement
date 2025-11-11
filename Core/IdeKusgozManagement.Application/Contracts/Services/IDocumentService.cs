using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.DocumentDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IDocumentService
    {
        Task<ApiResponse<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<DocumentTypeDTO>> GetDocumentTypeByIdAsync(string documentTypeId, CancellationToken cancellationToken = default);

        Task<ApiResponse<List<UserRequiredDocumentDTO>>> GetRequiredDocumentsAsync(string departmentId, string? targetId, CancellationToken cancellationToken = default);
    }
}