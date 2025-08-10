using IdeKusgozManagement.Application.DTOs.UserDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [RoleFilter("Admin")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Tüm kullanıcıları getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre kullanıcı getirir
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Yeni kullanıcı oluşturur
        /// </summary>
        /// <param name="createUserDTO">Kullanıcı bilgileri</param>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.CreateUserAsync(createUserDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcı bilgilerini günceller
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        /// <param name="updateUserDTO">Güncellenecek bilgiler</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserAsync(id, updateUserDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcıyı siler (soft delete)
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
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
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateUser(string id)
        {
            var result = await _userService.ActivateUserAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcıyı pasifleştirir
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        [HttpPost("{id}/deactivate")]
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