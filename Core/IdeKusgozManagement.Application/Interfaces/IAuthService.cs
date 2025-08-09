using IdeKusgozManagement.Application.DTOs.AuthDTOs;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDTO> LoginAsync(LoginDTO loginDTO);

        Task LogoutAsync();

        Task<TokenDTO> RefreshTokenAsync(CreateTokenByRefreshTokenDTO createTokenByRefreshTokenDTO);

        Task<bool> IsAuthenticatedAsync();
    }
}