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
        public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
        {
            var result = await _departmentApiService.GetDepartmentsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{departmentId}/gorev-liste")]
        public async Task<IActionResult> GetDepartmentDutiesByDepartment(string departmentId, CancellationToken cancellationToken)
        {
            var result = await _departmentApiService.GetDepartmentDutiesByDepartmentAsync(departmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}