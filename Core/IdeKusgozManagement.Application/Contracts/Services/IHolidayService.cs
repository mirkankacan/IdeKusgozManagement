using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.HolidayDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IHolidayService
    {
        Task<ServiceResponse<List<HolidayDTO>>> GetHolidaysByYearAsync(int year, CancellationToken cancellationToken = default);

        Task<ServiceResponse<double>> CalculateWorkingDaysAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}