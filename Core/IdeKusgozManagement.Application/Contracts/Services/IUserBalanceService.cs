using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserBalanceDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IUserBalanceService
    {
        Task<ServiceResponse<UserBalanceDTO>> GetUserBalancesByUserAsync(string userId, CancellationToken cancellationToken);

        Task<ServiceResponse<bool>> IncreaseUserBalanceAsync(string userId, UpdateUserBalanceDTO updateUserBalanceDTO, CancellationToken cancellationToken);

        Task<ServiceResponse<bool>> DecreaseUserBalanceAsync(string userId, UpdateUserBalanceDTO updateUserBalanceDTO, CancellationToken cancellationToken);
    }
}