using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.AuthModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IAuthApiService
    {
        Task<ApiResponse<TokenViewModel>> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> LogoutAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<TokenViewModel>> RefreshTokenAsync(CreateTokenByRefreshTokenViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> SendResetPasswordEmailAsync(ForgotPasswordViewModel model, CancellationToken cancellationToken = default);
    }
}