using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.CompanyModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class CompanyApiService : ICompanyApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<CompanyApiService> _logger;
        private const string BaseEndpoint = "api/companies";

        public CompanyApiService(
            IApiService apiService,
            ILogger<CompanyApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<CompanyViewModel>>> GetCompaniesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<CompanyViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<CompanyViewModel>> GetCompanyByIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<CompanyViewModel>($"{BaseEndpoint}/{companyId}", cancellationToken);
        }

        public async Task<ApiResponse<string>> CreateCompanyAsync(CreateCompanyViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<string>(endpoint: BaseEndpoint, model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> UpdateCompanyAsync(string companyId, UpdateCompanyViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>(endpoint: $"{BaseEndpoint}/{companyId}", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{companyId}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<CompanyViewModel>>> GetActiveCompaniesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<CompanyViewModel>>($"{BaseEndpoint}/active-companies", cancellationToken);
        }

        public async Task<ApiResponse<bool>> EnableCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{companyId}/enable", null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DisableCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{companyId}/disable", null, cancellationToken);
        }
    }
}