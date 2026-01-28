using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.HolidayModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class HolidayApiService : IHolidayApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<HolidayApiService> _logger;
        private const string BaseEndpoint = "api/holidays";

        public HolidayApiService(
            IApiService apiService,
            ILogger<HolidayApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<double>> CalculateWorkingDaysAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<double>($"{BaseEndpoint}/start/{startDate:yyyy-MM-dd}/end/{endDate:yyyy-MM-dd}", cancellationToken);
        }

        public async Task<ApiResponse<List<HolidayViewModel>>> GetHolidaysByYearAsync(int year, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<List<HolidayViewModel>>($"{BaseEndpoint}/{year}", cancellationToken);
        }
    }
}
