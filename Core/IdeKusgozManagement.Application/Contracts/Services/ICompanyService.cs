using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.CompanyDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface ICompanyService
    {
        Task<ServiceResult<IEnumerable<CompanyDTO>>> GetCompaniesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<CompanyDTO>> GetCompanyByIdAsync(string companyId, CancellationToken cancellationToken = default);

        Task<ServiceResult<string>> CreateCompanyAsync(CreateCompanyDTO createCompanyDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> UpdateCompanyAsync(string companyId, UpdateCompanyDTO updateCompanyDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteCompanyAsync(string companyId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<CompanyDTO>>> GetActiveCompaniesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> EnableCompanyAsync(string companyId, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DisableCompanyAsync(string companyId, CancellationToken cancellationToken = default);
    }
}