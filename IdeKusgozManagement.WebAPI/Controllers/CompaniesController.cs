using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [RoleFilter("Admin", "Yönetici", "Şef")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController(ICompanyService companyService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCompanies(CancellationToken cancellationToken)
        {
            var result = await companyService.GetCompaniesAsync(cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}