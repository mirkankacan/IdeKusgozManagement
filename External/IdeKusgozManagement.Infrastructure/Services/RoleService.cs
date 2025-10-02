using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.RoleDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<RoleService> logger) : IRoleService
    {
        public async Task<ApiResponse<bool>> IsUserInRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");

                var isInRole = await userManager.IsInRoleAsync(user, roleName);
                return ApiResponse<bool>.Success(isInRole);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "IsUserInRoleAsync işleminde hata oluştu. UserId:{userId} RoleName: {RoleName}", userId, roleName);
                return ApiResponse<bool>.Error("Kullanıcı rol kontolü yapılırken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<RoleDTO>>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var roles = await roleManager.Roles.ToListAsync();
                var orderedRoles = roles.OrderBy(role =>
                role.Name == "Admin" ? 1 :
                role.Name == "Yönetici" ? 2 :
                role.Name == "Şef" ? 3 :
                role.Name == "Personel" ? 4 : 5).ToList();
                var roleDTOs = orderedRoles.Select(role => role.Adapt<RoleDTO>()).ToList();

                return ApiResponse<IEnumerable<RoleDTO>>.Success(roleDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllRolesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<RoleDTO>>.Error("Roller getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<RoleDTO>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse<RoleDTO>.Error("Rol bulunamadı");
                }

                var roleDTO = role.Adapt<RoleDTO>();

                return ApiResponse<RoleDTO>.Success(roleDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRoleByIdAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                return ApiResponse<RoleDTO>.Error("Rol getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<RoleDTO>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return ApiResponse<RoleDTO>.Error("Rol bulunamadı");
                }

                var roleDTO = role.Adapt<RoleDTO>();

                return ApiResponse<RoleDTO>.Success(roleDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRoleByNameAsync işleminde hata oluştu. RoleName: {RoleName}", roleName);
                return ApiResponse<RoleDTO>.Error("Rol getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<RoleDTO>> CreateRoleAsync(CreateRoleDTO createRoleDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Rol adı kontrolü
                var existingRole = await roleManager.FindByNameAsync(createRoleDTO.Name);
                if (existingRole != null)
                {
                    return ApiResponse<RoleDTO>.Error("Bu rol adı zaten kullanılıyor");
                }

                var role = createRoleDTO.Adapt<ApplicationRole>();

                var result = await roleManager.CreateAsync(role);
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
                logger.LogError(ex, "CreateRoleAsync işleminde hata oluştu");
                return ApiResponse<RoleDTO>.Error("Rol oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<RoleDTO>> UpdateRoleAsync(string roleId, UpdateRoleDTO updateRoleDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse<RoleDTO>.Error("Rol bulunamadı");
                }

                // Rol adı değişikliği kontrolü
                if (role.Name != updateRoleDTO.Name)
                {
                    var existingRole = await roleManager.FindByNameAsync(updateRoleDTO.Name);
                    if (existingRole != null && existingRole.Id != roleId)
                    {
                        return ApiResponse<RoleDTO>.Error("Bu rol adı kullanılıyor");
                    }
                }

                // Mevcut role'u güncelle
                updateRoleDTO.Adapt(role);

                var result = await roleManager.UpdateAsync(role);
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
                logger.LogError(ex, "UpdateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                return ApiResponse<RoleDTO>.Error("Rol güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse<bool>.Error("Rol bulunamadı");
                }

                // Bu role sahip kullanıcı var mı kontrol et
                var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Any())
                {
                    return ApiResponse<bool>.Error("Bu role sahip kullanıcılar bulunduğu için rol silinemez");
                }

                // Soft delete
                role.IsActive = false;
                var result = await roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Rol başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteRoleAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                return ApiResponse<bool>.Error("Rol silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> EnableRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse<bool>.Error("Rol bulunamadı");
                }

                role.IsActive = true;
                var result = await roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Rol başarıyla aktifleştirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ActivateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                return ApiResponse<bool>.Error("Rol aktifleştirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DisableRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse<bool>.Error("Rol bulunamadı");
                }

                // Bu role sahip kullanıcı var mı kontrol et
                var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Any())
                {
                    return ApiResponse<bool>.Error("Bu role sahip kullanıcılar bulunduğu için rol pasifleştirilemez");
                }

                role.IsActive = false;
                var result = await roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Rol başarıyla pasifleştirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeactivateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                return ApiResponse<bool>.Error("Rol pasifleştirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<RoleDTO>>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var roles = await roleManager.Roles.Where(x => x.IsActive == true).ToListAsync();
                var orderedRoles = roles.OrderBy(role =>
                role.Name == "Admin" ? 1 :
                role.Name == "Yönetici" ? 2 :
                role.Name == "Şef" ? 3 :
                role.Name == "Personel" ? 4 : 5).ToList();
                var roleDTOs = orderedRoles.Select(role => role.Adapt<RoleDTO>()).ToList();

                return ApiResponse<IEnumerable<RoleDTO>>.Success(roleDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveRolesAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<RoleDTO>>.Error("Roller getirilirken hata oluştu");
            }
        }
    }
}