using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Yönetici, Şef")]
    [Route("dokuman")]
    public class DocumentController : Controller
    {
        private readonly IDocumentApiService _documentApiService;

        public DocumentController(IDocumentApiService documentApiService)
        {
            _documentApiService = documentApiService;
        }

        [HttpGet("yukle")]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpGet("yuklenenler")]
        public IActionResult UploadedDocuments()
        {
            return View();
        }

        [HttpGet("kontrol")]
        public IActionResult DocumentCheck()
        {
            return View();
        }

        [HttpGet("tip-liste")]
        public async Task<IActionResult> GetDocumentTypes(CancellationToken cancellationToken)
        {
            var result = await _documentApiService.GetDocumentTypesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{departmentDutyId}/tip-liste")]
        public async Task<IActionResult> GetDocumentTypesByDuty(string departmentDutyId, [FromQuery] string? companyId, CancellationToken cancellationToken)
        {
            var result = await _documentApiService.GetDocumentTypesByDutyAsync(departmentDutyId, companyId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("kontrol-liste")]
        public async Task<IActionResult> GetRequiredDocuments([FromQuery] string departmentId, [FromQuery] string departmentDutyId, [FromQuery] string? companyId, [FromQuery] string? targetId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(departmentId))
                return BadRequest("Departman ID'si gereklidir");
            if (string.IsNullOrEmpty(departmentDutyId))
                return BadRequest("Departman görev ID'si gereklidir");

            var result = await _documentApiService.GetRequiredDocumentsAsync(departmentId, departmentDutyId, companyId, targetId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}