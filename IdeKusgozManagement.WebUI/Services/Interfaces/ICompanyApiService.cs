using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.CompanyModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface ICompanyApiService
    {
        Task<ApiResponse<IEnumerable<CompanyViewModel>>> GetCompaniesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<CompanyViewModel>>> GetActiveCompaniesAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<CompanyViewModel>> GetCompanyByIdAsync(string companyId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateCompanyAsync(CreateCompanyViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateCompanyAsync(string companyId, UpdateCompanyViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteCompanyAsync(string companyId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> EnableCompanyAsync(string companyId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DisableCompanyAsync(string companyId, CancellationToken cancellationToken = default);
    }
}