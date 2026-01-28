using IdeKusgozManagement.WebUI.Extensions;
using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.UserModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Areas.Admin.Controllers
{
    [Route("kullanici")]
    public class UserController : Controller
    {
        private readonly IUserApiService _userApiService;
        private readonly IRoleApiService _roleApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IUserApiService userApiService, IRoleApiService roleApiService, IHttpContextAccessor httpContextAccessor)
        {
            _userApiService = userApiService;
            _roleApiService = roleApiService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("ana-liste")]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var response = await _userApiService.GetUsersAsync(cancellationToken);

            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetAssignedUsers(CancellationToken cancellationToken)
        {
            // Role claim'ini güvenli bir şekilde al
            var role = _httpContextAccessor.HttpContext?.Session.GetString("RoleName");
            if (string.IsNullOrEmpty(role))
            {
                return BadRequest("Rol bilgisi bulunamadı.");
            }

            ApiResponse<IEnumerable<UserViewModel>> response;

            switch (role)
            {
                case "Admin":

                    response = await _userApiService.GetUsersAsync(cancellationToken);
                    break;

                case "Yönetici":
                case "Şef":
                    var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
                    if (string.IsNullOrEmpty(userId))
                    {
                        return BadRequest("Kullanıcı ID'si bulunamadı.");
                    }
                    response = await _userApiService.GetSubordinatesByUserIdAsync(userId, cancellationToken);
                    break;

                default:
                    return Forbid("Yetkisiz erişim.");
            }

            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("aktif-ust-kullanicilar")]
        public async Task<IActionResult> GetActiveSuperiors(CancellationToken cancellationToken)
        {
            var response = await _userApiService.GetActiveSuperiorsAsync(cancellationToken);

            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici,Şef")]
        [HttpGet("departman/{departmentId}")]
        public async Task<IActionResult> GetUsersByDepartment(string departmentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var response = await _userApiService.GetUsersByDepartmentAsync(departmentId, cancellationToken);

            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici,Şef")]
        [HttpGet("departman-gorev/{departmentDutyId}")]
        public async Task<IActionResult> GetUsersByDepartmentDuty(string departmentDutyId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(departmentDutyId))
            {
                return BadRequest("Departman görev ID'si gereklidir");
            }
            var response = await _userApiService.GetUsersByDepartmentDutyAsync(departmentDutyId, cancellationToken);

            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("aktif-roller")]
        public async Task<IActionResult> GetActiveRoles(CancellationToken cancellationToken)
        {
            var response = await _roleApiService.GetActiveRolesAsync(cancellationToken);

            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost("olustur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.CreateUserAsync(model, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPut("guncelle/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.UpdateUserAsync(userId, model, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpDelete("sil/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var response = await _userApiService.DeleteUserAsync(userId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("aktif-et/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableUser(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var response = await _userApiService.EnableUserAsync(userId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("pasif-et/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableUser(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var response = await _userApiService.DisableUserAsync(userId, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost("rol-ata")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userApiService.AssignRoleToUserAsync(model, cancellationToken);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var response = await _userApiService.GetUserByIdAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response.Data) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("{userId}/kalan-izinler")]
        public async Task<IActionResult> GetAnnualLeaveByUser(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }

            var result = await _userApiService.GetAnnualLeaveDaysByUserAsync(userId, cancellationToken);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("kalan-izinlerim")]
        public async Task<IActionResult> GetMyAnnualLeave(CancellationToken cancellationToken)
        {
            var result = await _userApiService.GetMyAnnualLeaveDaysAsync(cancellationToken);
            return result.ToActionResult();
        }
    }
}