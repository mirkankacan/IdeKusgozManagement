using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.CompanyPaymentModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface ICompanyPaymentApiService
    {
        Task<ApiResponse<IEnumerable<CompanyPaymentViewModel>>> GetCompanyPaymentsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<CompanyPaymentViewModel>> GetCompanyPaymentByIdAsync(string companyPaymentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<CompanyPaymentViewModel>>> GetCompanyPaymentsByStatusAsync(int status, CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<CompanyPaymentViewModel>>> GetMyCompanyPaymentsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<IEnumerable<CompanyPaymentViewModel>>> GetCompanyPaymentsByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateCompanyPaymentAsync(CreateCompanyPaymentViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateCompanyPaymentAsync(string companyPaymentId, UpdateCompanyPaymentViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteCompanyPaymentAsync(string companyPaymentId, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> ApproveCompanyPaymentAsync(string companyPaymentId, string? chiefNote = null, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> RejectCompanyPaymentAsync(string companyPaymentId, string? rejectReason = null, CancellationToken cancellationToken = default);
    }
}
