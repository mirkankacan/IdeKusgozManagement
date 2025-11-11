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

        [HttpGet("dokuman-tip-liste")]
        public async Task<IActionResult> GetDocumentTypes(CancellationToken cancellationToken = default)
        {
            var result = await _documentApiService.GetDocumentTypesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dokuman-tip-liste/{departmentId}")]
        public async Task<IActionResult> GetDocumentTypesByDepartment(string departmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await _documentApiService.GetDocumentTypesByDepartmentAsync(departmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("gerekli-dokumanlar/p")]
        public async Task<IActionResult> GetUserRequiredDocuments([FromQuery] string departmentId, [FromQuery] string? targetId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await _documentApiService.GetRequiredDocumentsAsync(departmentId, targetId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}