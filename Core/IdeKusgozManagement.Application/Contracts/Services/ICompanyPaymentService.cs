using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.CompanyPaymentDTOs;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface ICompanyPaymentService
    {
        Task<ServiceResult<IEnumerable<CompanyPaymentDTO>>> GetCompanyPaymentsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<CompanyPaymentDTO>> GetCompanyPaymentByIdAsync(string companyPaymentId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<CompanyPaymentDTO>>> GetCompanyPaymentByStatusAsync(CompanyPaymentStatus status, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<CompanyPaymentDTO>>> GetCompanyPaymentsByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<ServiceResult<string>> CreateCompanyPaymentAsync(CreateCompanyPaymentDTO createCompanyPaymentDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> UpdateCompanyPaymentAsync(string companyPaymentId, UpdateCompanyPaymentDTO updateCompanyPaymentDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteCompanyPaymentAsync(string companyPaymentId, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> ApproveCompanyPaymentAsync(string companyPaymentId, string? chiefNote = null, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> RejectCompanyPaymentAsync(string companyPaymentId, string? rejectReason = null, CancellationToken cancellationToken = default);
    }
}