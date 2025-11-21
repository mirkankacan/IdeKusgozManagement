using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.CompanyDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface ICompanyService
    {
        Task<ServiceResponse<IEnumerable<CompanyDTO>>> GetCompaniesAsync(CancellationToken cancellationToken = default);
    }
}