using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.DepartmentModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class DepartmentApiService : IDepartmentApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<DepartmentApiService> _logger;
        private const string BaseEndpoint = "api/departments";

        public DepartmentApiService(
            IApiService apiService,
            ILogger<DepartmentApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<DepartmentDutyViewModel>>> GetDepartmentDutiesByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<DepartmentDutyViewModel>>($"{BaseEndpoint}/{departmentId}/duties", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<DepartmentViewModel>>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<DepartmentViewModel>>(BaseEndpoint, cancellationToken);
        }
    }
}