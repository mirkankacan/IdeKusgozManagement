using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.HolidayModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IHolidayApiService
    {
        Task<ApiResponse<List<HolidayViewModel>>> GetHolidaysByYear(int year, CancellationToken cancellationToken = default);
    }
}