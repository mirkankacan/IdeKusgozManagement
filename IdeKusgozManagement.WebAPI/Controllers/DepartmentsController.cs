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
    public class DepartmentsController(IDepartmentService departmentService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken = default)
        {
            var result = await departmentService.GetDepartmentsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("document-types")]
        public async Task<IActionResult> GetDocumentTypes(CancellationToken cancellationToken = default)
        {
            var result = await departmentService.GetDocumentTypesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("document-types-by-department/{departmentId}")]
        public async Task<IActionResult> GetDocumentTypesByDepartment(string departmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await departmentService.GetDocumentTypesByDepartmentAsync(departmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("relations-by-department/{departmentId}")]
        public async Task<IActionResult> GetDepartmentDocumentTypeRelationsByDepartment(string departmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await departmentService.GetDepartmentDocumentTypeRelationsByDepartmentAsync(departmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("relations")]
        public async Task<IActionResult> GetDepartmentDocumentTypeRelations(string departmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await departmentService.GetDepartmentDocumentTypeRelationsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}