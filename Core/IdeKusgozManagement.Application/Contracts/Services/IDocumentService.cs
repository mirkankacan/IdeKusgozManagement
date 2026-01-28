using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.DocumentDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IDocumentService
    {
        Task<ServiceResult<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<DocumentTypeDTO>>> GetDocumentTypesByDutyAsync(string departmentDutyId,/* string? companyId, */CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<RequiredDocumentDTO>>> GetRequiredDocumentsAsync(string departmentId, string departmentDutyId, string? companyId, string? targetId, string? documentTypeId, CancellationToken cancellationToken = default);

        // Servis içinde kullanılıyor API tarafında değil
        Task<ServiceResult<DocumentTypeDTO>> GetDocumentTypeByIdAsync(string documentTypeId, CancellationToken cancellationToken = default);

        Task<ServiceResult<string>> CreateDocumentTypeAsync(CreateDocumentTypeDTO createDocumentTypeDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> UpdateDocumentTypeAsync(string documentTypeId, UpdateDocumentTypeDTO updateDocumentTypeDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteDocumentTypeAsync(string documentTypeId, CancellationToken cancellationToken = default);

        Task<ServiceResult<string>> CreateDepartmentDocumentRequirmentAsync(CreateDepartmentDocumentRequirmentDTO createDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<DepartmentDocumentRequirmentDTO>>> GetDepartmentDocumentRequirmentsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteDepartmentDocumentRequirmentAsync(string requirementId, CancellationToken cancellationToken = default);
    }
}