using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.CompanyModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface ICompanyApiService
    {
        Task<ApiResponse<IEnumerable<CompanyViewModel>>> GetCompaniesAsync(CancellationToken cancellationToken = default);
    }
}