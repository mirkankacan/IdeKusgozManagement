using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AuthDTOs;

namespace IdeKusgozManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<TokenDTO>> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> LogoutAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<TokenDTO>> RefreshTokenAsync(CreateTokenByRefreshTokenDTO createTokenByRefreshTokenDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> SendResetPasswordEmailAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default);
    }
}