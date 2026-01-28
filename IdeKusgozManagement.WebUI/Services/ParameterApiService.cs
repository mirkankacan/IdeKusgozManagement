using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.ParameterModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class ParameterApiService : IParameterApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ParameterApiService> _logger;
        private const string BaseEndpoint = "api/parameters";

        public ParameterApiService(
            IApiService apiService,
            ILogger<ParameterApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> CreateParameterAsync(CreateParameterViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<string>(endpoint: BaseEndpoint, model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteParameterAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{id}", cancellationToken);
        }

        public async Task<ApiResponse<ParameterViewModel>> GetParameterByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<ParameterViewModel>($"{BaseEndpoint}/{key}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<ParameterViewModel>>> GetParametersAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<ParameterViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<bool>> UpdateParameterAsync(string id, UpdateParameterViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>(endpoint: $"{BaseEndpoint}/{id}", model, cancellationToken);
        }
    }
}