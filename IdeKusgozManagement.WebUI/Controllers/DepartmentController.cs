using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Yönetici, Şef")]
    [Route("departman")]
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

        [HttpGet("liste")]
        public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken = default)
        {
            var result = await _departmentApiService.GetDepartmentsAsync(cancellationToken);
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
        public async Task<IActionResult> GetDepartmentDocumentTypeRelations(CancellationToken cancellationToken = default)
        {
            var result = await _departmentApiService.GetDepartmentDocumentTypeRelationsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}