using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.EquipmentModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class EquipmentApiService : IEquipmentApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<EquipmentApiService> _logger;
        private const string BaseEndpoint = "api/equipments";

        public EquipmentApiService(
            IApiService apiService,
            ILogger<EquipmentApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<EquipmentViewModel>>> GetEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<EquipmentViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<EquipmentViewModel>> GetEquipmentByIdAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<EquipmentViewModel>($"{BaseEndpoint}/{equipmentId}", cancellationToken);
        }

        public async Task<ApiResponse<string>> CreateEquipmentAsync(CreateEquipmentViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<string>(BaseEndpoint, model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> UpdateEquipmentAsync(string equipmentId, UpdateEquipmentViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{equipmentId}", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{equipmentId}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<EquipmentViewModel>>> GetActiveEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<EquipmentViewModel>>($"{BaseEndpoint}/active-equipments", cancellationToken);
        }

        public async Task<ApiResponse<bool>> EnableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{equipmentId}/enable", null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DisableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{equipmentId}/disable", null, cancellationToken);
        }
    }
}
