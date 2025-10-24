using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AuthDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<TokenDTO>> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> LogoutAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<TokenDTO>> RefreshTokenAsync(CreateTokenByRefreshTokenDTO createTokenByRefreshTokenDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> SendResetPasswordEmailAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default);
    }
}