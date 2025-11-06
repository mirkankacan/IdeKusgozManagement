using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Yönetici, Şef")]
    [Route("")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentApiService _departmentApiService;

        public DepartmentController(IDepartmentApiService departmentApiService)
        {
            _departmentApiService = departmentApiService;
        }

        [HttpGet("evrak-yukle")]
        public IActionResult UploadDocument()
        {
            return View();
        }

        [HttpGet("departman-liste")]
        public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken = default)
        {
            var result = await _departmentApiService.GetDepartmentsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dokuman-tip-liste")]
        public async Task<IActionResult> GetDocumentTypes(CancellationToken cancellationToken = default)
        {
            var result = await _departmentApiService.GetDocumentTypesAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dokuman-tip-liste/{departmentId}")]
        public async Task<IActionResult> GetDocumentTypesByDepartment(string departmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await _departmentApiService.GetDocumentTypesByDepartmentAsync(departmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("iliskiler-liste/{departmentId}")]
        public async Task<IActionResult> GetDepartmentDocumentTypeRelationsByDepartment(string departmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await _departmentApiService.GetDepartmentDocumentTypeRelationsByDepartmentAsync(departmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("iliskiler-liste")]
        public async Task<IActionResult> GetDepartmentDocumentTypeRelations(string departmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await _departmentApiService.GetDepartmentDocumentTypeRelationsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}