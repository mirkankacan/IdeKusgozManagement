using IdeKusgozManagement.Application.DTOs.AuthDTOs;
using IdeKusgozManagement.Application.Interfaces;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        public Task<bool> IsAuthenticatedAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TokenDTO> LoginAsync(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TokenDTO> RefreshTokenAsync(CreateTokenByRefreshTokenDTO createTokenByRefreshTokenDTO)
        {
            throw new NotImplementedException();
        }
    }
}