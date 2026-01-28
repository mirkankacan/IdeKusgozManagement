using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserBalanceDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IUserBalanceService
    {
        Task<ServiceResult<UserBalanceDTO>> GetUserBalancesByUserAsync(string userId, CancellationToken cancellationToken);

        Task<ServiceResult<bool>> IncreaseUserBalanceAsync(string userId, UpdateUserBalanceDTO updateUserBalanceDTO, CancellationToken cancellationToken);

        Task<ServiceResult<bool>> DecreaseUserBalanceAsync(string userId, UpdateUserBalanceDTO updateUserBalanceDTO, CancellationToken cancellationToken);
    }
}