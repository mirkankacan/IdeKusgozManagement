using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.CompanyPaymentDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Enums;
using IdeKusgozManagement.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyPaymentsController(ICompanyPaymentService companyPaymentService, IIdentityService identityService) : ControllerBase
    {
        [Authorize(Policy = "FinansPolicy")]
        [HttpPut("{companyPaymentId}/approve")]
        public async Task<IActionResult> ApproveCompanyPayment(string companyPaymentId, [FromQuery] string? chiefNote = null)
        {
            if (string.IsNullOrWhiteSpace(companyPaymentId))
            {
                return BadRequest("Şirket ödemesi ID'si gereklidir");
            }

            var result = await companyPaymentService.ApproveCompanyPaymentAsync(companyPaymentId, chiefNote);
            return result.ToActionResult();
        }

        [Authorize(Policy = "FinansPolicy")]
        [HttpPut("{companyPaymentId}/reject")]
        public async Task<IActionResult> RejectCompanyPayment(string companyPaymentId, [FromQuery] string? rejectReason)
        {
            if (string.IsNullOrWhiteSpace(companyPaymentId))
            {
                return BadRequest("Şirket ödemesi ID'si gereklidir");
            }
            var result = await companyPaymentService.RejectCompanyPaymentAsync(companyPaymentId, rejectReason);
            return result.ToActionResult();
        }

        /// <summary>
        /// Tüm şirket ödemelerini getirir
        /// </summary>

        [Authorize(Policy = "FinansPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetCompanyPayments(CancellationToken cancellationToken)
        {
            var result = await companyPaymentService.GetCompanyPaymentsAsync(cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// ID'ye göre şirket ödemesi getirir
        /// </summary>
        /// <param name="companyPaymentId">şirket ödemesi ID'si</param>
        [HttpGet("{companyPaymentId}")]
        public async Task<IActionResult> GetCompanyPaymentById(string companyPaymentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyPaymentId))
            {
                return BadRequest("şirket ödemesi ID'si gereklidir");
            }

            var result = await companyPaymentService.GetCompanyPaymentByIdAsync(companyPaymentId, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Duruma göre şirket ödemelerini getirir
        /// </summary>
        /// <param name="status">şirket ödemesi durumu (Pending = 0, Approved = 1, Rejected = 2)</param>
        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetCompanyPaymentsByStatus(CompanyPaymentStatus status, CancellationToken cancellationToken)
        {
            var result = await companyPaymentService.GetCompanyPaymentByStatusAsync(status, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Kullanıcının kendi şirket ödemelerini getirir
        /// </summary>
        [HttpGet("my-company-payments")]
        public async Task<IActionResult> GetMyCompanyPayments(CancellationToken cancellationToken)
        {
            var userId = identityService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }
            var result = await companyPaymentService.GetCompanyPaymentsByUserIdAsync(userId, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Kullanıcı ID'sine göre şirket ödemelerini getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        [Authorize(Policy = "FinansPolicy")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCompanyPaymentsByUser(string userId, CancellationToken cancellationToken)
        {
            var result = await companyPaymentService.GetCompanyPaymentsByUserIdAsync(userId, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Yeni şirket ödemesi oluşturur
        /// </summary>
        /// <param name="createCompanyPaymentDTO">şirket ödemesi bilgileri</param>
        [HttpPost]
        public async Task<IActionResult> CreateCompanyPayment([FromForm] CreateCompanyPaymentDTO createCompanyPaymentDTO, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await companyPaymentService.CreateCompanyPaymentAsync(createCompanyPaymentDTO, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// şirket ödemesi günceller
        /// </summary>
        /// <param name="companyPaymentId">şirket ödemesi ID'si</param>
        /// <param name="updateCompanyPaymentDTO">Güncellenecek şirket ödemesi bilgileri</param>
        [HttpPut("{companyPaymentId}")]
        public async Task<IActionResult> UpdateCompanyPayment(string companyPaymentId, [FromForm] UpdateCompanyPaymentDTO updateCompanyPaymentDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyPaymentId))
            {
                return BadRequest("şirket ödemesi ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await companyPaymentService.UpdateCompanyPaymentAsync(companyPaymentId, updateCompanyPaymentDTO, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// şirket ödemesi siler
        /// </summary>
        /// <param name="companyPaymentId">şirket ödemesi ID'si</param>
        [HttpDelete("{companyPaymentId}")]
        public async Task<IActionResult> DeleteCompanyPayment(string companyPaymentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(companyPaymentId))
            {
                return BadRequest("şirket ödemesi ID'si gereklidir");
            }

            var result = await companyPaymentService.DeleteCompanyPaymentAsync(companyPaymentId, cancellationToken);
            return result.ToActionResult();
        }
    }
}