using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.DepartmentModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IDepartmentApiService
    {
        Task<ApiResponse<IEnumerable<DepartmentViewModel>>> GetDepartmentsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<DepartmentDutyViewModel>>> GetDepartmentDutiesByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default);
    }
}