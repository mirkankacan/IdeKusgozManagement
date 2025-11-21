using IdeKusgozManagement.Application.DTOs.UserDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService, IIdentityService identityService) : ControllerBase
    {
        /// <summary>
        /// Tüm kullanıcıları getirir
        /// </summary>
        [RoleFilter("Admin", "Yönetici")]
        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var result = await userService.GetUsersAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Departmana göre aktif kullanıcıları getirir
        /// </summary>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpGet("by-department/{departmentId}")]
        public async Task<IActionResult> GetUsersByDepartment(string departmentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Departman ID'si gereklidir");
            }
            var result = await userService.GetUsersByDepartmentAsync(departmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Admin, Şef, Yönetici rolüne sahip olan kullanıcıları getirir
        /// </summary>
        [RoleFilter("Admin", "Yönetici")]
        [HttpGet("active-superiors")]
        public async Task<IActionResult> GetActiveSuperiors()
        {
            var result = await userService.GetActiveSuperiorsAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre kendsine atanmış kullanıcıları getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpGet("subordiantes")]
        public async Task<IActionResult> GetSubordinatesByUserId([FromQuery] string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await userService.GetSubordinatesByUserIdAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre kullanıcı getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await userService.GetUserByIdAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Oturumda olan kullanıcının kendi bilgilerini getirir
        /// </summary>
        [HttpGet("my-user")]
        public async Task<IActionResult> GetMyUser(CancellationToken cancellationToken)
        {
            var currentUserId = identityService.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }
            var result = await userService.GetUserByIdAsync(currentUserId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni kullanıcı oluşturur
        /// </summary>
        /// <param name="createUserDTO">Kullanıcı bilgileri</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await userService.CreateUserAsync(createUserDTO, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Kullanıcı bilgilerini günceller
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="updateUserDTO">Güncellenecek bilgiler</param>
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDTO updateUserDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await userService.UpdateUserAsync(userId, updateUserDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcıyı siler
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await userService.DeleteUserAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcıya rol atar
        /// </summary>
        /// <param name="assignRoleDTO">Rol atama bilgileri</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO assignRoleDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await userService.AssignRoleToUserAsync(assignRoleDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcıyı aktifleştirir
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpPut("{userId}/enable")]
        public async Task<IActionResult> EnableUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await userService.EnableUserAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcıyı pasifleştirir
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpPut("{userId}/disable")]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await userService.DisableUserAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcının şifresini değiştirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="changePasswordDTO">Şifre değiştirme bilgileri</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpPost("{userId}/change-password")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await userService.ChangePasswordAsync(userId, changePasswordDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcının kalan izin günleri ile ilgili bilgileri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpGet("{userId}/annual-leave")]
        public async Task<IActionResult> GetAnnualLeaveByUser(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }

            var result = await userService.GetAnnualLeaveDaysByUserAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Oturumdaki kullanıcının kalan izin günleri ile ilgili bilgileri getirir
        /// </summary>
        [HttpGet("my-annual-leave")]
        public async Task<IActionResult> GetMyAnnualLeave(CancellationToken cancellationToken)
        {
            var userId = identityService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı. Lütfen tekrar giriş yapınız.");
            }

            var result = await userService.GetAnnualLeaveDaysByUserAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}