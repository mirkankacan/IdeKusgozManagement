using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.ProjectDTOs;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController(IProjectService projectService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetExpenses(CancellationToken cancellationToken = default)
        {
            var result = await projectService.GetProjectsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active-projects")]
        public async Task<IActionResult> GetActiveExpenses(CancellationToken cancellationToken = default)
        {
            var result = await projectService.GetActiveProjectsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpPut("{expenseId}/enable")]
        public async Task<IActionResult> EnableExpense(string expenseId)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }
            var result = await projectService.EnableProjectAsync(expenseId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpPut("{expenseId}/disable")]
        public async Task<IActionResult> DisableExpense(string expenseId)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }
            var result = await projectService.DisableProjectAsync(expenseId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{expenseId}")]
        public async Task<IActionResult> GetExpenseById(string expenseId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var result = await projectService.GetProjectByIdAsync(expenseId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> CreateExpense([FromBody] CreateProjectDTO createProjectDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await projectService.CreateProjectAsync(createProjectDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{expenseId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> UpdateExpense(string expenseId, [FromBody] UpdateProjectDTO updateProjectDTO, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await projectService.UpdateProjectAsync(expenseId, updateProjectDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{expenseId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> DeleteExpense(string expenseId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expenseId))
            {
                return BadRequest("Masraf türü ID'si gereklidir");
            }

            var result = await projectService.DeleteProjectAsync(expenseId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}