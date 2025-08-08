using IdeKusgozManagement.Application.DTOs.RoleDTOs;
using IdeKusgozManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
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
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre rol getirir
        /// </summary>
        /// <param name="id">Rol ID'si</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// İsme göre rol getirir
        /// </summary>
        /// <param name="name">Rol adı</param>
        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            var result = await _roleService.GetRoleByNameAsync(name);
            return result.IsSuccess ? Ok(result) : NotFound(result);
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
        /// <param name="id">Rol ID'si</param>
        /// <param name="updateRoleDTO">Güncellenecek bilgiler</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleDTO updateRoleDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _roleService.UpdateRoleAsync(id, updateRoleDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Rolü siler (soft delete)
        /// </summary>
        /// <param name="id">Rol ID'si</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Rolü aktifleştirir
        /// </summary>
        /// <param name="id">Rol ID'si</param>
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateRole(string id)
        {
            var result = await _roleService.ActivateRoleAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Rolü pasifleştirir
        /// </summary>
        /// <param name="id">Rol ID'si</param>
        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateRole(string id)
        {
            var result = await _roleService.DeactivateRoleAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Role sahip kullanıcıları getirir
        /// </summary>
        /// <param name="roleName">Rol adı</param>
        [HttpGet("{roleName}/users")]
        public async Task<IActionResult> GetUsersInRole(string roleName)
        {
            var result = await _roleService.GetUsersInRoleAsync(roleName);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}