using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.CompanyDTOs;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [RoleFilter("Admin", "Yönetici", "Şef")]
    [ApiController]
    public class CompaniesController(ICompanyService companyService) : ControllerBase
    {
        /// <summary>
        /// Tüm firmaları getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCompanies(CancellationToken cancellationToken)
        {
            var result = await companyService.GetCompaniesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Aktif tüm firmaları getirir
        /// </summary>
        [HttpGet("active-companies")]
        public async Task<IActionResult> GetActiveCompanies(CancellationToken cancellationToken)
        {
            var result = await companyService.GetActiveCompaniesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Firmayı aktifleştirir
        /// </summary>
        /// <param name="companyId">Firma ID'si</param>
        [HttpPut("{companyId}/enable")]
        public async Task<IActionResult> EnableCompany(string companyId)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return BadRequest("Firma ID'si gereklidir");
            }
            var result = await companyService.EnableCompanyAsync(companyId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Firmayı pasifleştirir
        /// </summary>
        /// <param name="companyId">Firma ID'si</param>
        [HttpPut("{companyId}/disable")]
        public async Task<IActionResult> DisableCompany(string companyId)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return BadRequest("Firma ID'si gereklidir");
            }
            var result = await companyService.DisableCompanyAsync(companyId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre firma getirir
        /// </summary>
        /// <param name="companyId">Firma ID'si</param>
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompanyById(string companyId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return BadRequest("Firma ID'si gereklidir");
            }

            var result = await companyService.GetCompanyByIdAsync(companyId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni firma oluşturur
        /// </summary>
        /// <param name="createCompanyDTO">Firma bilgileri</param>
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDTO createCompanyDTO, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await companyService.CreateCompanyAsync(createCompanyDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Firma günceller
        /// </summary>
        /// <param name="companyId">Firma ID'si</param>
        /// <param name="updateCompanyDTO">Güncellenecek firma bilgileri</param>
        [HttpPut("{companyId}")]
        public async Task<IActionResult> UpdateCompany(string companyId, [FromBody] UpdateCompanyDTO updateCompanyDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return BadRequest("Firma ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await companyService.UpdateCompanyAsync(companyId, updateCompanyDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Firma siler
        /// </summary>
        /// <param name="companyId">Firma ID'si</param>
        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteCompany(string companyId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return BadRequest("Firma ID'si gereklidir");
            }

            var result = await companyService.DeleteCompanyAsync(companyId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}