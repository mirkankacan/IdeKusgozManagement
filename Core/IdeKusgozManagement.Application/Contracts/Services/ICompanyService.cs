using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.CompanyDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface ICompanyService
    {
        Task<ServiceResponse<IEnumerable<CompanyDTO>>> GetCompaniesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<CompanyDTO>> GetCompanyByIdAsync(string companyId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<string>> CreateCompanyAsync(CreateCompanyDTO createCompanyDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> UpdateCompanyAsync(string companyId, UpdateCompanyDTO updateCompanyDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteCompanyAsync(string companyId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<IEnumerable<CompanyDTO>>> GetActiveCompaniesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> EnableCompanyAsync(string companyId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DisableCompanyAsync(string companyId, CancellationToken cancellationToken = default);
    }
}