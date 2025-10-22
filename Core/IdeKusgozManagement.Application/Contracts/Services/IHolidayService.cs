using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.HolidayDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IHolidayService
    {
        Task<ApiResponse<List<HolidayDTO>>> GetHolidaysByYearAsync(int year, CancellationToken cancellationToken = default);

        Task<ApiResponse<double>> CalculateWorkingDaysAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}