using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.DocumentModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class DocumentApiService : IDocumentApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<DocumentApiService> _logger;
        private const string BaseEndpoint = "api/documents";

        public DocumentApiService(
            IApiService apiService,
            ILogger<DocumentApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<DocumentTypeViewModel>>($"{BaseEndpoint}/types", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<DocumentTypeViewModel>>> GetDocumentTypesByDutyAsync(string departmentDutyId, /*string? companyId, */CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<DocumentTypeViewModel>>($"{BaseEndpoint}/{departmentDutyId}/types", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<RequiredDocumentViewModel>>> GetRequiredDocumentsAsync(string departmentId, string departmentDutyId, string? companyId, string? targetId, string? documentTypeId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<RequiredDocumentViewModel>>($"{BaseEndpoint}/check?departmentId={departmentId}&departmentDutyId={departmentDutyId}&companyId={companyId}&targetId={targetId}&documentTypeId={documentTypeId}", cancellationToken);
        }

        public async Task<ApiResponse<DocumentTypeViewModel>> GetDocumentTypeByIdAsync(string documentTypeId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<DocumentTypeViewModel>($"{BaseEndpoint}/types/{documentTypeId}", cancellationToken);
        }

        public async Task<ApiResponse<string>> CreateDocumentTypeAsync(CreateDocumentTypeViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<string>($"{BaseEndpoint}/types", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> UpdateDocumentTypeAsync(string documentTypeId, UpdateDocumentTypeViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/types/{documentTypeId}", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteDocumentTypeAsync(string documentTypeId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/types/{documentTypeId}", cancellationToken);
        }

        public async Task<ApiResponse<string>> CreateDepartmentDocumentRequirmentAsync(CreateDepartmentDocumentRequirmentViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<string>($"{BaseEndpoint}/requirements", model, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<DepartmentDocumentRequirmentViewModel>>> GetDepartmentDocumentRequirmentsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<DepartmentDocumentRequirmentViewModel>>($"{BaseEndpoint}/requirements", cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteDepartmentDocumentRequirmentAsync(string requirementId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/requirements/{requirementId}", cancellationToken);
        }
    }
}