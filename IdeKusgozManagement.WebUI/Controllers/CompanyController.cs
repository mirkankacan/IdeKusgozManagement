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
            var result = await _companyApiService.GetCompaniesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        //[Authorize]
        //[ValidateAntiForgeryToken]
        //[HttpPost("olustur")]
        //public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyViewModel model, CancellationToken cancellationToken)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var response = await _companyApiService.GetCompaniesAsync(model, cancellationToken);
        //    return response.IsSuccess ? Ok(response) : BadRequest(response);
        //}
    }
}