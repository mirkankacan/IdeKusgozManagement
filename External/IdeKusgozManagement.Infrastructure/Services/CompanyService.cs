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
    public class CompanyService(IUnitOfWork unitOfWork, ILogger<CompanyService> logger) : ICompanyService
    {
        public async Task<ServiceResponse<IEnumerable<CompanyDTO>>> GetCompaniesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var companies = (await unitOfWork.GetRepository<IdtCompany>().GetAllAsync(cancellationToken)).OrderByDescending(c => c.CreatedDate);

                var companyDTOs = companies.Adapt<IEnumerable<CompanyDTO>>();

                return ServiceResponse<IEnumerable<CompanyDTO>>.Success(companyDTOs, "Firma listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetCompaniesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<CompanyDTO>> GetCompanyByIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await unitOfWork.GetRepository<IdtCompany>().GetFirstOrDefaultAsync(c => c.Id == companyId, cancellationToken);

                if (company == null)
                {
                    return ServiceResponse<CompanyDTO>.Error("Firma bulunamadı");
                }

                var companyDTO = company.Adapt<CompanyDTO>();

                return ServiceResponse<CompanyDTO>.Success(companyDTO, "Firma başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetCompanyByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<string>> CreateCompanyAsync(CreateCompanyDTO createCompanyDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingCompany = await unitOfWork.GetRepository<IdtCompany>().AnyAsync(c => c.Name.ToLower() == createCompanyDTO.Name.ToLower(), cancellationToken);

                if (existingCompany)
                {
                    return ServiceResponse<string>.Error("Bu isimde bir firma zaten mevcut");
                }

                var company = createCompanyDTO.Adapt<IdtCompany>();
                company.IsActive = true;
                await unitOfWork.GetRepository<IdtCompany>().AddAsync(company, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResponse<string>.Success(company.Id, "Firma başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateCompanyAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> UpdateCompanyAsync(string companyId, UpdateCompanyDTO updateCompanyDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await unitOfWork.GetRepository<IdtCompany>().GetByIdAsync(companyId, cancellationToken);

                if (company == null)
                {
                    return ServiceResponse<bool>.Error("Firma bulunamadı");
                }

                var existingCompany = await unitOfWork.GetRepository<IdtCompany>().AnyAsync(c => c.Name.ToLower() == updateCompanyDTO.Name.ToLower() && c.Id != companyId, cancellationToken);

                if (existingCompany)
                {
                    return ServiceResponse<bool>.Error("Bu isimde başka bir firma zaten mevcut");
                }

                updateCompanyDTO.Adapt(company);

                unitOfWork.GetRepository<IdtCompany>().Update(company);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Firma başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateCompanyAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await unitOfWork.GetRepository<IdtCompany>().GetByIdAsync(companyId, cancellationToken);

                if (company == null)
                {
                    return ServiceResponse<bool>.Error("Firma bulunamadı");
                }

                var isCompanyUsed = await unitOfWork.GetRepository<IdtFile>().AnyAsync(f => f.TargetCompanyId == company.Id, cancellationToken);

                if (isCompanyUsed)
                {
                    return ServiceResponse<bool>.Error("Bu firma dosya kayıtlarında kullanıldığı için silinemez");
                }

                unitOfWork.GetRepository<IdtCompany>().Remove(company);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Firma başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteCompanyAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<CompanyDTO>>> GetActiveCompaniesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var companies = await unitOfWork.GetRepository<IdtCompany>().Where(c => c.IsActive == true).OrderBy(c => c.Name).ToListAsync(cancellationToken);

                var companyDTOs = companies.Adapt<IEnumerable<CompanyDTO>>();

                return ServiceResponse<IEnumerable<CompanyDTO>>.Success(companyDTOs, "Aktif firma listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveCompaniesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DisableCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await unitOfWork.GetRepository<IdtCompany>().GetByIdAsync(companyId, cancellationToken);

                if (company == null)
                {
                    return ServiceResponse<bool>.Error("Firma bulunamadı");
                }

                company.IsActive = false;

                unitOfWork.GetRepository<IdtCompany>().Update(company);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Firma başarıyla pasif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableCompanyAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> EnableCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await unitOfWork.GetRepository<IdtCompany>().GetByIdAsync(companyId, cancellationToken);

                if (company == null)
                {
                    return ServiceResponse<bool>.Error("Firma bulunamadı");
                }

                company.IsActive = true;

                unitOfWork.GetRepository<IdtCompany>().Update(company);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Firma başarıyla aktif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableCompanyAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}