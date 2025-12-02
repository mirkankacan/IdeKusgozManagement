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
        [HttpGet("types")]
        public async Task<IActionResult> GetDocumentTypes(CancellationToken cancellationToken)
        {
            var result = await documentService.GetDocumentTypesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{departmentDutyId}/types")]
        public async Task<IActionResult> GetDocumentTypesByDuty(string departmentDutyId,/* [FromQuery] string? companyId, */CancellationToken cancellationToken)
        {
            var result = await documentService.GetDocumentTypesByDutyAsync(departmentDutyId, /*companyId,*/ cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("check")]
        public async Task<IActionResult> GetRequiredDocuments([FromQuery] string departmentId, [FromQuery] string departmentDutyId, [FromQuery] string? companyId, [FromQuery] string? targetId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(departmentId))
                return BadRequest("Departman ID'si gereklidir");
            if (string.IsNullOrEmpty(departmentDutyId))
                return BadRequest("Departman görev ID'si gereklidir");

            var result = await documentService.GetRequiredDocumentsAsync(departmentId, departmentDutyId, companyId, targetId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}