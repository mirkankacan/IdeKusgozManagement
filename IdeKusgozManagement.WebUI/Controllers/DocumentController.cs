using IdeKusgozManagement.WebUI.Models.DocumentModels;
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

        [HttpGet("kontrol")]
        public IActionResult DocumentCheck()
        {
            return View();
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("eslestirme")]
        public IActionResult DocumentMatch()
        {
            return View();
        }

        [HttpGet("tip-liste")]
        public async Task<IActionResult> GetDocumentTypes(CancellationToken cancellationToken)
        {
            var result = await _documentApiService.GetDocumentTypesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("tip/{documentTypeId}")]
        public async Task<IActionResult> GetDocumentTypeById(string documentTypeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(documentTypeId))
            {
                return BadRequest("Doküman tipi ID'si gereklidir");
            }

            var response = await _documentApiService.GetDocumentTypeByIdAsync(documentTypeId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("tip")]
        public async Task<IActionResult> CreateDocumentType([FromBody] CreateDocumentTypeViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _documentApiService.CreateDocumentTypeAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [ValidateAntiForgeryToken]
        [HttpPut("tip/{documentTypeId}")]
        public async Task<IActionResult> UpdateDocumentType(string documentTypeId, [FromBody] UpdateDocumentTypeViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(documentTypeId))
            {
                return BadRequest("Doküman tipi ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _documentApiService.UpdateDocumentTypeAsync(documentTypeId, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [ValidateAntiForgeryToken]
        [HttpDelete("tip/{documentTypeId}")]
        public async Task<IActionResult> DeleteDocumentType(string documentTypeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(documentTypeId))
            {
                return BadRequest("Doküman tipi ID'si gereklidir");
            }

            var response = await _documentApiService.DeleteDocumentTypeAsync(documentTypeId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("eslestir")]
        public async Task<IActionResult> CreateDepartmentDocumentRequirment([FromBody] CreateDepartmentDocumentRequirmentViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _documentApiService.CreateDepartmentDocumentRequirmentAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("eslestirme-liste")]
        public async Task<IActionResult> GetDepartmentDocumentRequirments(CancellationToken cancellationToken)
        {
            var result = await _documentApiService.GetDepartmentDocumentRequirmentsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [ValidateAntiForgeryToken]
        [HttpDelete("eslestirme/{requirementId}")]
        public async Task<IActionResult> DeleteDepartmentDocumentRequirment(string requirementId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(requirementId))
            {
                return BadRequest("Gereklilik ID'si gereklidir");
            }

            var response = await _documentApiService.DeleteDepartmentDocumentRequirmentAsync(requirementId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{departmentDutyId}/tip-liste")]
        public async Task<IActionResult> GetDocumentTypesByDuty(string departmentDutyId, /*[FromQuery] string? companyId,*/ CancellationToken cancellationToken)
        {
            var result = await _documentApiService.GetDocumentTypesByDutyAsync(departmentDutyId, /*companyId,*/ cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("kontrol-liste")]
        public async Task<IActionResult> GetRequiredDocuments([FromQuery] string departmentId, [FromQuery] string departmentDutyId, [FromQuery] string? companyId, [FromQuery] string? targetId, [FromQuery] string? documentTypeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(departmentId))
                return BadRequest("Departman ID'si gereklidir");
            if (string.IsNullOrEmpty(departmentDutyId))
                return BadRequest("Departman görev ID'si gereklidir");

            var result = await _documentApiService.GetRequiredDocumentsAsync(departmentId, departmentDutyId, companyId, targetId, documentTypeId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}