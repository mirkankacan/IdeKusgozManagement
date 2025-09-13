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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Tüm kullanıcıları getirir
        /// </summary>
        [RoleFilter("Admin", "Yönetici")]
        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken = default)
        {
            var result = await _userService.GetUsersAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Admin, Şef, Yönetici rolüne sahip olan kullanıcıları getirir
        /// </summary>
        [RoleFilter("Admin", "Yönetici")]
        [HttpGet("active-superiors")]
        public async Task<IActionResult> GetActiveSuperiors()
        {
            var result = await _userService.GetActiveSuperiorsAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre kendsine atanmış kullanıcıları getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpGet("subordiantes")]
        public async Task<IActionResult> GetSubordinatesByUserId([FromQuery] string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await _userService.GetSubordinatesByUserIdAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre kullanıcı getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await _userService.GetUserByIdAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Oturumda olan kullanıcının kendi bilgilerini getirir
        /// </summary>
        [HttpGet("my-user")]
        public async Task<IActionResult> GetMyUser(CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }
            var result = await _userService.GetUserByIdAsync(currentUserId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni kullanıcı oluşturur
        /// </summary>
        /// <param name="createUserDTO">Kullanıcı bilgileri</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.CreateUserAsync(createUserDTO, cancellationToken);

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
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDTO updateUserDTO, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserAsync(userId, updateUserDTO, cancellationToken);
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
            var result = await _userService.DeleteUserAsync(userId);
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

            var result = await _userService.AssignRoleToUserAsync(assignRoleDTO);
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
            var result = await _userService.EnableUserAsync(userId);
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
            var result = await _userService.DisableUserAsync(userId);
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

            var result = await _userService.ChangePasswordAsync(userId, changePasswordDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}