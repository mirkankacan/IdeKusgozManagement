using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.AuthModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class AuthApiService : IAuthApiService
    {
        public Task<ApiResponse<bool>> CheckAuthAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<TokenViewModel>> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> LogoutAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<TokenViewModel>> RefreshTokenAsync(CreateTokenByRefreshTokenViewModel model, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}