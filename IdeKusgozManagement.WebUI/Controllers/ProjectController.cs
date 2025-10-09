using IdeKusgozManagement.WebUI.Models.ProjectModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("proje")]
    public class ProjectController : Controller
    {
        private readonly IProjectApiService _projectApiService;

        public ProjectController(IProjectApiService projectApiService)
        {
            _projectApiService = projectApiService;
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetProjects(CancellationToken cancellationToken = default)
        {
            var response = await _projectApiService.GetProjectsAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("aktif-liste")]
        public async Task<IActionResult> GetActiveProjects(CancellationToken cancellationToken = default)
        {
            var response = await _projectApiService.GetActiveProjectsAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById(string projectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Proje ID'si gereklidir");
            }

            var response = await _projectApiService.GetProjectByIdAsync(projectId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPost("")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _projectApiService.CreateProjectAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject(string projectId, [FromBody] UpdateProjectViewModel model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Proje ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _projectApiService.UpdateProjectAsync(projectId, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(string projectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Proje ID'si gereklidir");
            }

            var response = await _projectApiService.DeleteProjectAsync(projectId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{projectId}/aktif-et")]
        public async Task<IActionResult> EnableProject(string projectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Proje ID'si gereklidir");
            }

            var response = await _projectApiService.EnableProjectAsync(projectId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{projectId}/pasif-et")]
        public async Task<IActionResult> DisableProject(string projectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Proje ID'si gereklidir");
            }

            var response = await _projectApiService.DisableProjectAsync(projectId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}