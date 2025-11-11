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
    public class DocumentsController(IDocumentService documentService) : ControllerBase
    {
        [HttpGet("document-types")]
        public async Task<IActionResult> GetDocumentTypes(CancellationToken cancellationToken = default)
        {
            var result = await documentService.GetDocumentTypesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("document-types-by-department/{departmentId}")]
        public async Task<IActionResult> GetDocumentTypesByDepartment(string departmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await documentService.GetDocumentTypesByDepartmentAsync(departmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("check/by-params")]
        public async Task<IActionResult> GetUserRequiredDocuments([FromQuery] string departmentId, [FromQuery] string? targetId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await documentService.GetRequiredDocumentsAsync(departmentId, targetId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}