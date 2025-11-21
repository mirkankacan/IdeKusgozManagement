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
        public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
        {
            var result = await departmentService.GetDepartmentsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{departmentId}/duties")]
        public async Task<IActionResult> GetDepartmentDutiesByDepartment(string departmentId, CancellationToken cancellationToken)
        {
            var result = await departmentService.GetDepartmentDutiesByDepartmentAsync(departmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        //[HttpGet("duty/document/relations")]
        //public async Task<IActionResult> GetDepartmentDutyDocumentRelations([FromQuery] string? departmentId, [FromQuery] string? departmentDutyId, [FromQuery] string? companyId, CancellationToken cancellationToken )
        //{
        //    var result = await departmentService.GetDepartmentDutyDocumentRelationsAsync(departmentId, departmentDutyId, companyId, cancellationToken);
        //    return result.IsSuccess ? Ok(result) : BadRequest(result);
        //}
    }
}