using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AuthDTOs;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<TokenDTO>> LoginAsync(LoginDTO loginDTO);

        Task<ApiResponse<bool>> LogoutAsync();

        Task<ApiResponse<TokenDTO>> RefreshTokenAsync(CreateTokenByRefreshTokenDTO createTokenByRefreshTokenDTO);

        Task<ApiResponse<bool>> IsAuthenticatedAsync();
    }
}