using IdeKusgozManagement.Application.DTOs.AuthDTOs;
using IdeKusgozManagement.Domain.Entities;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IJwtProvider
    {
        Task<TokenDTO> CreateTokenAsync(ApplicationUser applicationUser);
    }
}