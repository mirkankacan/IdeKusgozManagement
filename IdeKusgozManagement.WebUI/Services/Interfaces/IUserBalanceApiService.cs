using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.UserBalanceModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IUserBalanceApiService
    {
        Task<ApiResponse<UserBalanceViewModel>> GetUserBalancesByUserAsync(string userId, CancellationToken cancellationToken);

        Task<ApiResponse<UserBalanceViewModel>> GetMyBalancesAsync(CancellationToken cancellationToken);

        Task<ApiResponse<bool>> IncreaseUserBalanceAsync(string userId, UpdateUserBalanceViewModel model, CancellationToken cancellationToken);

        Task<ApiResponse<bool>> DecreaseUserBalanceAsync(string userId, UpdateUserBalanceViewModel model, CancellationToken cancellationToken);
    }
}