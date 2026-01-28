using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.CompanyDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class CompanyService(IUnitOfWork unitOfWork, ILogger<CompanyService> logger) : ICompanyService
    {
        public async Task<ServiceResult<IEnumerable<CompanyDTO>>> GetCompaniesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var companies = (await unitOfWork.GetRepository<IdtCompany>().GetAllAsync(cancellationToken)).OrderByDescending(c => c.CreatedDate);

                var companyDTOs = companies.Adapt<IEnumerable<CompanyDTO>>();

                return ServiceResult<IEnumerable<CompanyDTO>>.SuccessAsOk(companyDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetCompaniesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<CompanyDTO>> GetCompanyByIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await unitOfWork.GetRepository<IdtCompany>().GetFirstOrDefaultAsync(c => c.Id == companyId, cancellationToken);

                if (company == null)
                {
                    return ServiceResult<CompanyDTO>.Error("Firma Bulunamadı", "Belirtilen ID'ye sahip firma bulunamadı.", HttpStatusCode.NotFound);
                }

                var companyDTO = company.Adapt<CompanyDTO>();

                return ServiceResult<CompanyDTO>.SuccessAsOk(companyDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetCompanyByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<string>> CreateCompanyAsync(CreateCompanyDTO createCompanyDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingCompany = await unitOfWork.GetRepository<IdtCompany>().AnyAsync(c => c.Name.ToLower() == createCompanyDTO.Name.ToLower(), cancellationToken);

                if (existingCompany)
                {
                    return ServiceResult<string>.Error("Firma Zaten Mevcut", "Bu isimde bir firma zaten mevcut. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                var company = createCompanyDTO.Adapt<IdtCompany>();
                company.IsActive = true;
                await unitOfWork.GetRepository<IdtCompany>().AddAsync(company, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResult<string>.SuccessAsCreated(company.Id, $"/api/companies/{company.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateCompanyAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> UpdateCompanyAsync(string companyId, UpdateCompanyDTO updateCompanyDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await unitOfWork.GetRepository<IdtCompany>().GetByIdAsync(companyId, cancellationToken);

                if (company == null)
                {
                    return ServiceResult<bool>.Error("Firma Bulunamadı", "Belirtilen ID'ye sahip firma bulunamadı.", HttpStatusCode.NotFound);
                }

                var existingCompany = await unitOfWork.GetRepository<IdtCompany>().AnyAsync(c => c.Name.ToLower() == updateCompanyDTO.Name.ToLower() && c.Id != companyId, cancellationToken);

                if (existingCompany)
                {
                    return ServiceResult<bool>.Error("Firma Zaten Mevcut", "Bu isimde başka bir firma zaten mevcut. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                updateCompanyDTO.Adapt(company);

                unitOfWork.GetRepository<IdtCompany>().Update(company);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateCompanyAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await unitOfWork.GetRepository<IdtCompany>().GetByIdAsync(companyId, cancellationToken);

                if (company == null)
                {
                    return ServiceResult<bool>.Error("Firma Bulunamadı", "Belirtilen ID'ye sahip firma bulunamadı.", HttpStatusCode.NotFound);
                }

                var isCompanyUsed = await unitOfWork.GetRepository<IdtFile>().AnyAsync(f => f.TargetCompanyId == company.Id, cancellationToken);

                if (isCompanyUsed)
                {
                    return ServiceResult<bool>.Error("Silme İşlemi Başarısız", "Bu firma dosya kayıtlarında kullanıldığı için silinemez.", HttpStatusCode.BadRequest);
                }

                unitOfWork.GetRepository<IdtCompany>().Remove(company);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteCompanyAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<CompanyDTO>>> GetActiveCompaniesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var companies = await unitOfWork.GetRepository<IdtCompany>().Where(c => c.IsActive == true).OrderBy(c => c.Name).ToListAsync(cancellationToken);

                var companyDTOs = companies.Adapt<IEnumerable<CompanyDTO>>();

                return ServiceResult<IEnumerable<CompanyDTO>>.SuccessAsOk(companyDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveCompaniesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DisableCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await unitOfWork.GetRepository<IdtCompany>().GetByIdAsync(companyId, cancellationToken);

                if (company == null)
                {
                    return ServiceResult<bool>.Error("Firma Bulunamadı", "Belirtilen ID'ye sahip firma bulunamadı.", HttpStatusCode.NotFound);
                }

                company.IsActive = false;

                unitOfWork.GetRepository<IdtCompany>().Update(company);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableCompanyAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> EnableCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await unitOfWork.GetRepository<IdtCompany>().GetByIdAsync(companyId, cancellationToken);

                if (company == null)
                {
                    return ServiceResult<bool>.Error("Firma Bulunamadı", "Belirtilen ID'ye sahip firma bulunamadı.", HttpStatusCode.NotFound);
                }

                company.IsActive = true;

                unitOfWork.GetRepository<IdtCompany>().Update(company);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableCompanyAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}