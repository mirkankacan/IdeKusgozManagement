using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.UserBalanceModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class UserBalanceApiService : IUserBalanceApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<UserBalanceApiService> _logger;
        private const string BaseEndpoint = "api/userbalances";

        public UserBalanceApiService(
            IApiService apiService,
            ILogger<UserBalanceApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> DecreaseUserBalanceAsync(string userId, UpdateUserBalanceViewModel model, CancellationToken cancellationToken)
        {
            return await _apiService.PostAsync<bool>($"{BaseEndpoint}/{userId}/decrease", model, cancellationToken);
        }

        public async Task<ApiResponse<UserBalanceViewModel>> GetMyBalancesAsync(CancellationToken cancellationToken)
        {
            return await _apiService.GetAsync<UserBalanceViewModel>($"{BaseEndpoint}/my-balance", cancellationToken);
        }

        public async Task<ApiResponse<UserBalanceViewModel>> GetUserBalancesByUserAsync(string userId, CancellationToken cancellationToken)
        {
            return await _apiService.GetAsync<UserBalanceViewModel>($"{BaseEndpoint}/{userId}", cancellationToken);
        }

        public async Task<ApiResponse<bool>> IncreaseUserBalanceAsync(string userId, UpdateUserBalanceViewModel model, CancellationToken cancellationToken)
        {
            return await _apiService.PostAsync<bool>($"{BaseEndpoint}/{userId}/increase", model, cancellationToken);
        }
    }
}
