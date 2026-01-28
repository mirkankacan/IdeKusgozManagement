using IdeKusgozManagement.WebUI.Extensions;
using IdeKusgozManagement.WebUI.Models.CompanyPaymentModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("sirket-odemesi")]
    public class CompanyPaymentController : Controller
    {
        private readonly ICompanyPaymentApiService _companyPaymentApiService;
        private readonly IHttpContextAccessor _contextAccessor;

        public CompanyPaymentController(ICompanyPaymentApiService companyPaymentApiService, IHttpContextAccessor contextAccessor)
        {
            _companyPaymentApiService = companyPaymentApiService;
            _contextAccessor = contextAccessor;
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("")]
        public IActionResult Index()
        {
            var userId = _contextAccessor.HttpContext.Session.GetString("UserId");
            ViewBag.UserId = userId;
            return View();
        }

        [HttpGet("olustur")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetCompanyPayments(CancellationToken cancellationToken)
        {
            var response = await _companyPaymentApiService.GetCompanyPaymentsAsync(cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("{companyPaymentId}")]
        public async Task<IActionResult> GetCompanyPaymentById(string companyPaymentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyPaymentId))
            {
                return BadRequest("Şirket ödemesi ID'si gereklidir");
            }

            var response = await _companyPaymentApiService.GetCompanyPaymentByIdAsync(companyPaymentId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("durum/{status}")]
        public async Task<IActionResult> GetCompanyPaymentsByStatus(int status, CancellationToken cancellationToken)
        {
            var response = await _companyPaymentApiService.GetCompanyPaymentsByStatusAsync(status, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("listem")]
        public async Task<IActionResult> GetMyCompanyPayments(CancellationToken cancellationToken)
        {
            var response = await _companyPaymentApiService.GetMyCompanyPaymentsAsync(cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("kullanici/{userId}")]
        public async Task<IActionResult> GetCompanyPaymentsByUser(string userId, CancellationToken cancellationToken)
        {
            var response = await _companyPaymentApiService.GetCompanyPaymentsByUserIdAsync(userId, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost("olustur")]
        public async Task<IActionResult> Create([FromForm] CreateCompanyPaymentViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _companyPaymentApiService.CreateCompanyPaymentAsync(model, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPut("{companyPaymentId}")]
        public async Task<IActionResult> UpdateCompanyPayment(string companyPaymentId, [FromForm] UpdateCompanyPaymentViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyPaymentId))
            {
                return BadRequest("Şirket ödemesi ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _companyPaymentApiService.UpdateCompanyPaymentAsync(companyPaymentId, model, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpDelete("{companyPaymentId}")]
        public async Task<IActionResult> DeleteCompanyPayment(string companyPaymentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyPaymentId))
            {
                return BadRequest("Şirket ödemesi ID'si gereklidir");
            }

            var response = await _companyPaymentApiService.DeleteCompanyPaymentAsync(companyPaymentId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [ValidateAntiForgeryToken]
        [HttpPut("{companyPaymentId}/onayla")]
        public async Task<IActionResult> ApproveCompanyPayment(string companyPaymentId, [FromQuery] string? chiefNote = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(companyPaymentId))
            {
                return BadRequest("Şirket ödemesi ID'si gereklidir");
            }

            var response = await _companyPaymentApiService.ApproveCompanyPaymentAsync(companyPaymentId, chiefNote, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [ValidateAntiForgeryToken]
        [HttpPut("{companyPaymentId}/reddet")]
        public async Task<IActionResult> RejectCompanyPayment(string companyPaymentId, [FromQuery] string? rejectReason = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(companyPaymentId))
            {
                return BadRequest("Şirket ödemesi ID'si gereklidir");
            }

            var response = await _companyPaymentApiService.RejectCompanyPaymentAsync(companyPaymentId, rejectReason, cancellationToken);
            return response.ToActionResult();
        }
    }
}