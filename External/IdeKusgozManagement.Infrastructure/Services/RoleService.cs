using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.RoleDTOs;
using IdeKusgozManagement.Application.DTOs.UserDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<RoleDTO>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var roleDTOs = roles.Select(role => role.Adapt<RoleDTO>()).ToList();

                return ApiResponse<IEnumerable<RoleDTO>>.Success(roleDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllRolesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<RoleDTO>>.Error("Roller getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<RoleDTO>> GetRoleByIdAsync(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return ApiResponse<RoleDTO>.Error("Rol bulunamadı");
                }

                var roleDTO = role.Adapt<RoleDTO>();

                return ApiResponse<RoleDTO>.Success(roleDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRoleByIdAsync işleminde hata oluştu. RoleId: {RoleId}", id);
                return ApiResponse<RoleDTO>.Error("Rol getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<RoleDTO>> GetRoleByNameAsync(string name)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(name);
                if (role == null)
                {
                    return ApiResponse<RoleDTO>.Error("Rol bulunamadı");
                }

                var roleDTO = role.Adapt<RoleDTO>();

                return ApiResponse<RoleDTO>.Success(roleDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRoleByNameAsync işleminde hata oluştu. RoleName: {RoleName}", name);
                return ApiResponse<RoleDTO>.Error("Rol getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<RoleDTO>> CreateRoleAsync(CreateRoleDTO createRoleDTO)
        {
            try
            {
                // Rol adı kontrolü
                var existingRole = await _roleManager.FindByNameAsync(createRoleDTO.Name);
                if (existingRole != null)
                {
                    return ApiResponse<RoleDTO>.Error("Bu rol adı zaten kullanılıyor");
                }

                var role = createRoleDTO.Adapt<ApplicationRole>();

                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<RoleDTO>.Error(errors);
                }

                var roleDTO = role.Adapt<RoleDTO>();

                return ApiResponse<RoleDTO>.Success(roleDTO, "Rol başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateRoleAsync işleminde hata oluştu");
                return ApiResponse<RoleDTO>.Error("Rol oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<RoleDTO>> UpdateRoleAsync(string id, UpdateRoleDTO updateRoleDTO)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return ApiResponse<RoleDTO>.Error("Rol bulunamadı");
                }

                // Rol adı değişikliği kontrolü
                if (role.Name != updateRoleDTO.Name)
                {
                    var existingRole = await _roleManager.FindByNameAsync(updateRoleDTO.Name);
                    if (existingRole != null && existingRole.Id != id)
                    {
                        return ApiResponse<RoleDTO>.Error("Bu rol adı zaten kullanılıyor");
                    }
                }

                // Mevcut role'u güncelle
                updateRoleDTO.Adapt(role);

                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<RoleDTO>.Error(errors);
                }

                var roleDTO = role.Adapt<RoleDTO>();

                return ApiResponse<RoleDTO>.Success(roleDTO, "Rol başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", id);
                return ApiResponse<RoleDTO>.Error("Rol güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return ApiResponse<bool>.Error("Rol bulunamadı");
                }

                // Bu role sahip kullanıcı var mı kontrol et
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Any())
                {
                    return ApiResponse<bool>.Error("Bu role sahip kullanıcılar bulunduğu için rol silinemez");
                }

                // Soft delete
                role.IsActive = false;
                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Rol başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteRoleAsync işleminde hata oluştu. RoleId: {RoleId}", id);
                return ApiResponse<bool>.Error("Rol silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> ActivateRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse<bool>.Error("Rol bulunamadı");
                }

                role.IsActive = true;
                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Rol başarıyla aktifleştirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActivateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                return ApiResponse<bool>.Error("Rol aktifleştirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeactivateRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse<bool>.Error("Rol bulunamadı");
                }

                // Bu role sahip kullanıcı var mı kontrol et
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Any())
                {
                    return ApiResponse<bool>.Error("Bu role sahip kullanıcılar bulunduğu için rol pasifleştirilemez");
                }

                role.IsActive = false;
                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Rol başarıyla pasifleştirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                return ApiResponse<bool>.Error("Rol pasifleştirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
                var userDTOs = usersInRole.Select(user =>
                {
                    var userDto = user.Adapt<UserDTO>();
                    userDto.Role = roleName;
                    return userDto;
                }).ToList();

                return ApiResponse<IEnumerable<UserDTO>>.Success(userDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUsersInRoleAsync işleminde hata oluştu. RoleName: {RoleName}", roleName);
                return ApiResponse<IEnumerable<UserDTO>>.Error("Role sahip kullanıcılar getirilirken hata oluştu");
            }
        }
    }
}