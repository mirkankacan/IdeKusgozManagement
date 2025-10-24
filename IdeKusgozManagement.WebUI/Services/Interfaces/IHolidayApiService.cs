using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.HolidayModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IHolidayApiService
    {
        Task<ApiResponse<List<HolidayViewModel>>> GetHolidaysByYearAsync(int year, CancellationToken cancellationToken = default);

        Task<ApiResponse<double>> CalculateWorkingDaysAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}