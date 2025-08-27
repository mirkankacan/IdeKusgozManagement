using System.Security.Claims;
using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.UserModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Areas.Admin.Controllers
{
    [Route("kullanici-yonetimi")]
    public class UserManagementController : Controller
    {
        private readonly IUserApiService _userApiService;
        private readonly IRoleApiService _roleApiService;

        public UserManagementController(IUserApiService userApiService, IRoleApiService roleApiService)
        {
            _userApiService = userApiService;
            _roleApiService = roleApiService;
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken = default)
        {
            // Role claim'ini güvenli bir şekilde al
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            if (roleClaim == null)
            {
                return BadRequest("Rol bilgisi bulunamadı.");
            }

            ApiResponse<IEnumerable<UserViewModel>> response;

            switch (roleClaim.Value)
            {
                case "Admin":
                case "Yönetici":
                    response = await _userApiService.GetAllUsersAsync(cancellationToken);
                    break;

                case "Şef":
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                    {
                        return BadRequest("Kullanıcı ID'si bulunamadı.");
                    }
                    response = await _userApiService.GetAssignedUsersByIdAsync(userId, cancellationToken);
                    break;

                default:
                    return Forbid("Yetkisiz erişim.");
            }

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("aktif-ust-kullanicilar")]
        public async Task<IActionResult> GetActiveSuperiorUsers(CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.GetActiveSuperiorUsersAsync(cancellationToken);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("aktif-roller")]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken = default)
        {
            var response = await _roleApiService.GetActiveRolesAsync(cancellationToken);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost("olustur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateUserViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.CreateUserAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPut("guncelle/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string userId, [FromBody] UpdateUserViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.UpdateUserAsync(userId, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpDelete("sil/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.DeleteUserAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost("aktif/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.ActivateUserAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost("pasif/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.DeactivateUserAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost("rol-ata")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.AssignRoleToUserAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("detay/{userId}")]
        public async Task<IActionResult> Get(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.GetUserByIdAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response.Data) : BadRequest(response);
        }
    }
}