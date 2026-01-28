using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.RoleDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<RoleService> logger) : IRoleService
    {
        public async Task<ServiceResult<bool>> IsUserInRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                    return ServiceResult<bool>.Error("Kullanıcı Bulunamadı", "Belirtilen ID'ye sahip kullanıcı bulunamadı.", HttpStatusCode.NotFound);

                var isInRole = await userManager.IsInRoleAsync(user, roleName);
                return ServiceResult<bool>.SuccessAsOk(isInRole);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "IsUserInRoleAsync işleminde hata oluştu. UserId:{userId} RoleName: {RoleName}", userId, roleName);
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<RoleDTO>>> GetRolesAsync(CancellationToken cancellationToken = default)
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

                return ServiceResult<IEnumerable<RoleDTO>>.SuccessAsOk(roleDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllRolesAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<RoleDTO>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ServiceResult<RoleDTO>.Error("Rol Bulunamadı", "Belirtilen ID'ye sahip rol bulunamadı.", HttpStatusCode.NotFound);
                }

                var roleDTO = role.Adapt<RoleDTO>();

                return ServiceResult<RoleDTO>.SuccessAsOk(roleDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRoleByIdAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<ServiceResult<RoleDTO>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return ServiceResult<RoleDTO>.Error("Rol Bulunamadı", "Belirtilen isme sahip rol bulunamadı.", HttpStatusCode.NotFound);
                }

                var roleDTO = role.Adapt<RoleDTO>();

                return ServiceResult<RoleDTO>.SuccessAsOk(roleDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRoleByNameAsync işleminde hata oluştu. RoleName: {RoleName}", roleName);
                throw;
            }
        }

        public async Task<ServiceResult<RoleDTO>> CreateRoleAsync(CreateRoleDTO createRoleDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Rol adı kontrolü
                var existingRole = await roleManager.FindByNameAsync(createRoleDTO.Name);
                if (existingRole != null)
                {
                    return ServiceResult<RoleDTO>.Error("Rol Adı Zaten Kullanılıyor", "Bu rol adı zaten kullanılıyor. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                var role = createRoleDTO.Adapt<ApplicationRole>();

                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<RoleDTO>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                }

                var roleDTO = role.Adapt<RoleDTO>();

                return ServiceResult<RoleDTO>.SuccessAsCreated(roleDTO, $"/api/roles/{roleDTO.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateRoleAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<RoleDTO>> UpdateRoleAsync(string roleId, UpdateRoleDTO updateRoleDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ServiceResult<RoleDTO>.Error("Rol Bulunamadı", "Belirtilen ID'ye sahip rol bulunamadı.", HttpStatusCode.NotFound);
                }

                // Rol adı değişikliği kontrolü
                if (role.Name != updateRoleDTO.Name)
                {
                    var existingRole = await roleManager.FindByNameAsync(updateRoleDTO.Name);
                    if (existingRole != null && existingRole.Id != roleId)
                    {
                        return ServiceResult<RoleDTO>.Error("Rol Adı Kullanılıyor", "Bu rol adı kullanılıyor. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                    }
                }

                // Mevcut role'u güncelle
                updateRoleDTO.Adapt(role);

                var result = await roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<RoleDTO>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                }

                var roleDTO = role.Adapt<RoleDTO>();

                return ServiceResult<RoleDTO>.SuccessAsOk(roleDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ServiceResult<bool>.Error("Rol Bulunamadı", "Belirtilen ID'ye sahip rol bulunamadı.", HttpStatusCode.NotFound);
                }

                // Bu role sahip kullanıcı var mı kontrol et
                var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Any())
                {
                    return ServiceResult<bool>.Error("Silme İşlemi Başarısız", "Bu role sahip kullanıcılar bulunduğu için rol silinemez.", HttpStatusCode.BadRequest);
                }

                // Soft delete
                role.IsActive = false;
                var result = await roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<bool>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                }

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteRoleAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> EnableRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ServiceResult<bool>.Error("Rol Bulunamadı", "Belirtilen ID'ye sahip rol bulunamadı.", HttpStatusCode.NotFound);
                }

                role.IsActive = true;
                var result = await roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<bool>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                }

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ActivateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DisableRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ServiceResult<bool>.Error("Rol Bulunamadı", "Belirtilen ID'ye sahip rol bulunamadı.", HttpStatusCode.NotFound);
                }

                // Bu role sahip kullanıcı var mı kontrol et
                var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Any())
                {
                    return ServiceResult<bool>.Error("Pasifleştirme İşlemi Başarısız", "Bu role sahip kullanıcılar bulunduğu için rol pasifleştirilemez.", HttpStatusCode.BadRequest);
                }

                role.IsActive = false;
                var result = await roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<bool>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                }

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeactivateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<RoleDTO>>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
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

                return ServiceResult<IEnumerable<RoleDTO>>.SuccessAsOk(roleDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveRolesAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}