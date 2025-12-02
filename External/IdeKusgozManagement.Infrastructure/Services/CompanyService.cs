using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.CompanyDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class CompanyService(IUnitOfWork unitOfWork, ILogger<DepartmentService> logger) : ICompanyService
    {
        public async Task<ServiceResponse<IEnumerable<CompanyDTO>>> GetCompaniesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var companies = await unitOfWork.GetRepository<IdtCompany>()
                    .WhereAsNoTracking(x => x.Id != null)
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                if (companies == null)
                {
                    return ServiceResponse<IEnumerable<CompanyDTO>>.Success(null, "Firmalar bulunamadı");
                }

                var mappedCompanies = companies.Adapt<IEnumerable<CompanyDTO>>();

                return ServiceResponse<IEnumerable<CompanyDTO>>.Success(mappedCompanies, "Firmalar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetCompaniesAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}