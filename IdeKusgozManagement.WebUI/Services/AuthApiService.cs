using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.AuthModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class AuthApiService : IAuthApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthApiService> _logger;
        private readonly ILogger<ApiService> _apiServiceLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string BaseEndpoint = "api/auth";

        public AuthApiService(
            IHttpClientFactory httpClientFactory,
            ILogger<AuthApiService> logger,
            ILogger<ApiService> apiServiceLogger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiServiceLogger = apiServiceLogger;
            _httpContextAccessor = httpContextAccessor;
        }

        private IApiService CreateApiService(string clientName)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            return new ApiService(httpClient, _apiServiceLogger, _httpContextAccessor);
        }

        public async Task<ApiResponse<TokenViewModel>> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default)
        {
            var apiService = CreateApiService("AuthApiWithoutToken");
            return await apiService.PostAsync<TokenViewModel>($"{BaseEndpoint}/login", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> LogoutAsync(CancellationToken cancellationToken = default)
        {
            var apiService = CreateApiService("AuthApiWithToken");
            return await apiService.PostAsync<bool>($"{BaseEndpoint}/logout", null, cancellationToken);
        }

        public async Task<ApiResponse<TokenViewModel>> RefreshTokenAsync(CreateTokenByRefreshTokenViewModel model, CancellationToken cancellationToken = default)
        {
            var apiService = CreateApiService("AuthApiWithoutToken");
            return await apiService.PostAsync<TokenViewModel>($"{BaseEndpoint}/refresh-token", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordViewModel model, CancellationToken cancellationToken = default)
        {
            var apiService = CreateApiService("AuthApiWithoutToken");
            return await apiService.PostAsync<bool>($"{BaseEndpoint}/reset-password", model, cancellationToken);
        }

        public async Task<ApiResponse<bool>> SendResetPasswordEmailAsync(ForgotPasswordViewModel model, CancellationToken cancellationToken = default)
        {
            var apiService = CreateApiService("AuthApiWithoutToken");
            return await apiService.PostAsync<bool>($"{BaseEndpoint}/send-reset-email", model, cancellationToken);
        }
    }
}