using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.DocumentDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IDocumentService
    {
        Task<ServiceResponse<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesByDutyAsync(string departmentDutyId,/* string? companyId, */CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<RequiredDocumentDTO>>> GetRequiredDocumentsAsync(string departmentId, string departmentDutyId, string? companyId, string? targetId, string? documentTypeId, CancellationToken cancellationToken = default);

        // Servis içinde kullanılıyor API tarafında değil
        Task<ServiceResponse<DocumentTypeDTO>> GetDocumentTypeByIdAsync(string documentTypeId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<string>> CreateDocumentTypeAsync(CreateDocumentTypeDTO createDocumentTypeDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> UpdateDocumentTypeAsync(string documentTypeId, UpdateDocumentTypeDTO updateDocumentTypeDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteDocumentTypeAsync(string documentTypeId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<string>> CreateDepartmentDocumentRequirmentAsync(CreateDepartmentDocumentRequirmentDTO createDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<DepartmentDocumentRequirmentDTO>>> GetDepartmentDocumentRequirmentsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteDepartmentDocumentRequirmentAsync(string requirementId, CancellationToken cancellationToken = default);
    }
}