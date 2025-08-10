using IdeKusgozManagement.WebUI.Models.UserModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("admin/kullanici-yonetimi")]
    public class UserManagementController : Controller // Typo düzeltildi: UserManagment -> UserManagement
    {
        private readonly IUserApiService _userApiService;
        private readonly IRoleApiService _roleApiService;
        public UserManagementController(IUserApiService userApiService, IRoleApiService roleApiService)
        {
            _userApiService = userApiService;
            _roleApiService = roleApiService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("liste")]
        public async Task<IActionResult> GetUsersForDataTable(CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.GetAllUsersAsync(cancellationToken);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("roller")]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken = default)
        {
            var response = await _roleApiService.GetAllRolesAsync(cancellationToken);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
        [HttpPost("olustur")]
        public async Task<IActionResult> Create([FromBody] CreateUserViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.CreateUserAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPut("guncelle/{userId}")]
        public async Task<IActionResult> Update(string userId, [FromBody]  UpdateUserViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.UpdateUserAsync(userId, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("sil/{userId}")]
        public async Task<IActionResult> Delete(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.DeleteUserAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("aktif/{userId}")]
        public async Task<IActionResult> Activate(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.ActivateUserAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("pasif/{userId}")]
        public async Task<IActionResult> Deactivate(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.DeactivateUserAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("rol-ata")]
        public async Task<IActionResult> AssignRole([FromBody]  AssignRoleViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.AssignRoleToUserAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("detay/{userId}")]
        public async Task<IActionResult> Get(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.GetUserByIdAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response.Data) : BadRequest(response);
        }
    }
}