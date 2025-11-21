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
        public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
        {
            var result = await projectService.GetProjectsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active-projects")]
        public async Task<IActionResult> GetActiveProjects(CancellationToken cancellationToken)
        {
            var result = await projectService.GetActiveProjectsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpPut("{projectId}/enable")]
        public async Task<IActionResult> EnableProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Proje ID'si gereklidir");
            }
            var result = await projectService.EnableProjectAsync(projectId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpPut("{projectId}/disable")]
        public async Task<IActionResult> DisableProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Proje ID'si gereklidir");
            }
            var result = await projectService.DisableProjectAsync(projectId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById(string projectId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Proje ID'si gereklidir");
            }

            var result = await projectService.GetProjectByIdAsync(projectId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDTO createProjectDTO, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await projectService.CreateProjectAsync(createProjectDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{projectId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> UpdateProject(string projectId, [FromBody] UpdateProjectDTO updateProjectDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Proje ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await projectService.UpdateProjectAsync(projectId, updateProjectDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{projectId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> DeleteProject(string projectId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Proje ID'si gereklidir");
            }

            var result = await projectService.DeleteProjectAsync(projectId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}