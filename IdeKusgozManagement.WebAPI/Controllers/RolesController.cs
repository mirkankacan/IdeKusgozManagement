using IdeKusgozManagement.Application.DTOs.RoleDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [RoleFilter("Admin")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Tüm rolleri getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _roleService.GetRolesAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tüm aktif rolleri getirir
        /// </summary>
        [HttpGet("active-roles")]
        public async Task<IActionResult> GetActiveRoles()
        {
            var result = await _roleService.GetActiveRolesAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre rol getirir
        /// </summary>
        /// <param name="roleId">Rol ID'si</param>
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Rol ID'si gereklidir");
            }
            var result = await _roleService.GetRoleByIdAsync(roleId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İsme göre rol getirir
        /// </summary>
        /// <param name="name">Rol adı</param>
        [HttpGet("name/{roleName}")]
        public async Task<IActionResult> GetRoleByName(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Rol adı gereklidir");
            }
            var result = await _roleService.GetRoleByNameAsync(roleName);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni rol oluşturur
        /// </summary>
        /// <param name="createRoleDTO">Rol bilgileri</param>
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO createRoleDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _roleService.CreateRoleAsync(createRoleDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Rol bilgilerini günceller
        /// </summary>
        /// <param name="roleId">Rol ID'si</param>
        /// <param name="updateRoleDTO">Güncellenecek bilgiler</param>
        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] UpdateRoleDTO updateRoleDTO)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Rol ID'si gereklidir");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _roleService.UpdateRoleAsync(roleId, updateRoleDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Rolü siler (soft delete)
        /// </summary>
        /// <param name="id">Rol ID'si</param>
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Rol ID'si gereklidir");
            }
            var result = await _roleService.DeleteRoleAsync(roleId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Rolü aktifleştirir
        /// </summary>
        /// <param name="id">Rol ID'si</param>
        [HttpPost("{roleId}/enable")]
        public async Task<IActionResult> EnableRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Rol ID'si gereklidir");
            }
            var result = await _roleService.EnableRoleAsync(roleId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Rolü pasifleştirir
        /// </summary>
        /// <param name="id">Rol ID'si</param>
        [HttpPost("{roleId}/disable")]
        public async Task<IActionResult> DisableRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Rol ID'si gereklidir");
            }
            var result = await _roleService.DisableRoleAsync(roleId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}