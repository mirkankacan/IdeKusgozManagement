using IdeKusgozManagement.Application.DTOs.UserDTOs;
using IdeKusgozManagement.Application.Interfaces;
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
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken = default)
        {
            var result = await _userService.GetAllUsersAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Admin, Şef, Yönetici rolüne sahip olan kullanıcıları getirir
        /// </summary>
        [RoleFilter("Admin", "Yönetici")]
        [HttpGet("active-superiors")]
        public async Task<IActionResult> GetActiveSuperiorUsers()
        {
            var result = await _userService.GetActiveSuperiorUsersAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre kendsine atanmış kullanıcıları getirir
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpGet("assigned-users")]
        public async Task<IActionResult> GetAssignedUsersById([FromQuery] string id, CancellationToken cancellationToken = default)
        {
            var result = await _userService.GetAssignedUsersByIdAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// ID'ye göre kullanıcı getirir
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id, CancellationToken cancellationToken = default)
        {
            var result = await _userService.GetUserByIdAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result);
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
            return result.IsSuccess ? Ok(result) : NotFound(result);
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
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcı bilgilerini günceller
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        /// <param name="updateUserDTO">Güncellenecek bilgiler</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO updateUserDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserAsync(id, updateUserDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcıyı siler (soft delete)
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
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
        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateUser(string id)
        {
            var result = await _userService.ActivateUserAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcıyı pasifleştirir
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        [RoleFilter("Admin", "Yönetici")]
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            var result = await _userService.DeactivateUserAsync(id);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.ChangePasswordAsync(userId, changePasswordDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}