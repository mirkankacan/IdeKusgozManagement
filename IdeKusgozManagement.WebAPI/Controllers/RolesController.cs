using IdeKusgozManagement.Application.DTOs.RoleDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Infrastructure.Authorization;
using IdeKusgozManagement.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        /// <summary>
        /// Tüm rolleri getirir
        /// </summary>
        [HttpGet]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await roleService.GetRolesAsync();
            return result.ToActionResult();
        }

        /// <summary>
        /// Tüm aktif rolleri getirir
        /// </summary>
        [HttpGet("active-roles")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> GetActiveRoles()
        {
            var result = await roleService.GetActiveRolesAsync();
            return result.ToActionResult();
        }

        /// <summary>
        /// ID'ye göre rol getirir
        /// </summary>
        /// <param name="roleId">Rol ID'si</param>
        [HttpGet("{roleId}")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Rol ID'si gereklidir");
            }
            var result = await roleService.GetRoleByIdAsync(roleId);
            return result.ToActionResult();
        }

        /// <summary>
        /// İsme göre rol getirir
        /// </summary>
        /// <param name="name">Rol adı</param>
        [HttpGet("name/{roleName}")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> GetRoleByName(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Rol adı gereklidir");
            }
            var result = await roleService.GetRoleByNameAsync(roleName);
            return result.ToActionResult();
        }

        /// <summary>
        /// Yeni rol oluşturur
        /// </summary>
        /// <param name="createRoleDTO">Rol bilgileri</param>
        [HttpPost]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO createRoleDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await roleService.CreateRoleAsync(createRoleDTO);
            return result.ToActionResult();
        }

        /// <summary>
        /// Rol bilgilerini günceller
        /// </summary>
        /// <param name="roleId">Rol ID'si</param>
        /// <param name="updateRoleDTO">Güncellenecek bilgiler</param>
        [HttpPut("{roleId}")]
        [RoleFilter("Admin", "Yönetici")]
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

            var result = await roleService.UpdateRoleAsync(roleId, updateRoleDTO);
            return result.ToActionResult();
        }

        /// <summary>
        /// Rolü siler (soft delete)
        /// </summary>
        /// <param name="id">Rol ID'si</param>
        [HttpDelete("{roleId}")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Rol ID'si gereklidir");
            }
            var result = await roleService.DeleteRoleAsync(roleId);
            return result.ToActionResult();
        }

        /// <summary>
        /// Rolü aktifleştirir
        /// </summary>
        /// <param name="id">Rol ID'si</param>
        [HttpPost("{roleId}/enable")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> EnableRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Rol ID'si gereklidir");
            }
            var result = await roleService.EnableRoleAsync(roleId);
            return result.ToActionResult();
        }

        /// <summary>
        /// Rolü pasifleştirir
        /// </summary>
        /// <param name="id">Rol ID'si</param>
        [HttpPost("{roleId}/disable")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> DisableRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Rol ID'si gereklidir");
            }
            var result = await roleService.DisableRoleAsync(roleId);
            return result.ToActionResult();
        }
    }
}