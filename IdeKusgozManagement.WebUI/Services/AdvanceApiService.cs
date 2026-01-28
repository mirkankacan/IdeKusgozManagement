using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.AdvanceModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class AdvanceApiService : IAdvanceApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<AdvanceApiService> _logger;
        private const string BaseEndpoint = "api/advances";

        public AdvanceApiService(
            IApiService apiService,
            ILogger<AdvanceApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<AdvanceStatisticViewModel>> GetAdvanceStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<AdvanceStatisticViewModel>($"{BaseEndpoint}/statistics", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<AdvanceViewModel>>> GetAdvancesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<AdvanceViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<AdvanceViewModel>>> GetMyAdvancesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<AdvanceViewModel>>($"{BaseEndpoint}/my-advances", cancellationToken);
        }

        public async Task<ApiResponse<AdvanceViewModel>> GetAdvanceByIdAsync(string advanceId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<AdvanceViewModel>($"{BaseEndpoint}/{advanceId}", cancellationToken);
        }

        public async Task<ApiResponse<string>> CreateAdvanceAsync(CreateAdvanceViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PostAsync<string>(BaseEndpoint, model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> UpdateAdvanceAsync(string advanceId, UpdateAdvanceViewModel model, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{advanceId}", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteAdvanceAsync(string advanceId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{advanceId}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<AdvanceViewModel>>> GetAdvancesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<AdvanceViewModel>>($"{BaseEndpoint}/user/{userId}", cancellationToken);
        }

        public async Task<ApiResponse<bool>> ApproveAdvanceAsync(string advanceId, ApproveAdvanceViewModel? model = null, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{advanceId}/approve", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> RejectAdvanceAsync(string advanceId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{advanceId}/reject";
            if (!string.IsNullOrEmpty(rejectReason))
            {
                endpoint += $"?rejectReason={Uri.EscapeDataString(rejectReason)}";
            }
            return await _apiService.PutAsync<bool>(endpoint, null, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<AdvanceViewModel>>> GetApprovedAdvancesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<AdvanceViewModel>>($"{BaseEndpoint}/approved", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<AdvanceViewModel>>> GetCompletedAdvancesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<AdvanceViewModel>>($"{BaseEndpoint}/completed", cancellationToken);
        }

        public async Task<ApiResponse<bool>> CompleteAdvanceAsync(string advanceId, ApproveAdvanceViewModel? model = null, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{advanceId}/complete", model, cancellationToken);
        }
    }
}