using IdeKusgozManagement.WebUI.Extensions;
using IdeKusgozManagement.WebUI.Models.CompanyModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Yönetici, Şef")]
    [Route("firma")]
    public class CompanyController : Controller
    {
        private readonly ICompanyApiService _companyApiService;

        public CompanyController(ICompanyApiService companyApiService)
        {
            _companyApiService = companyApiService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("liste")]
        public async Task<IActionResult> GetCompanies(CancellationToken cancellationToken)
        {
            var response = await _companyApiService.GetCompaniesAsync(cancellationToken);
            return response.ToActionResult();
        }

        [HttpGet("aktif-liste")]
        public async Task<IActionResult> GetActiveCompanies(CancellationToken cancellationToken)
        {
            var response = await _companyApiService.GetActiveCompaniesAsync(cancellationToken);
            return response.ToActionResult();
        }

        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompanyById(string companyId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return BadRequest("Firma ID'si gereklidir");
            }

            var response = await _companyApiService.GetCompanyByIdAsync(companyId, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [HttpPost("")]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _companyApiService.CreateCompanyAsync(model, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [HttpPut("{companyId}")]
        public async Task<IActionResult> UpdateCompany(string companyId, [FromBody] UpdateCompanyViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return BadRequest("Firma ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _companyApiService.UpdateCompanyAsync(companyId, model, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteCompany(string companyId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return BadRequest("Firma ID'si gereklidir");
            }

            var response = await _companyApiService.DeleteCompanyAsync(companyId, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [HttpPut("{companyId}/aktif-et")]
        public async Task<IActionResult> EnableCompany(string companyId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return BadRequest("Firma ID'si gereklidir");
            }

            var response = await _companyApiService.EnableCompanyAsync(companyId, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [HttpPut("{companyId}/pasif-et")]
        public async Task<IActionResult> DisableCompany(string companyId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyId))
            {
                return BadRequest("Firma ID'si gereklidir");
            }

            var response = await _companyApiService.DisableCompanyAsync(companyId, cancellationToken);
            return response.ToActionResult();
        }
    }
}