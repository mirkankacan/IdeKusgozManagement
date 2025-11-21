using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AuthDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<TokenDTO>> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> LogoutAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<TokenDTO>> RefreshTokenAsync(CreateTokenByRefreshTokenDTO createTokenByRefreshTokenDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> SendResetPasswordEmailAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default);
    }
}