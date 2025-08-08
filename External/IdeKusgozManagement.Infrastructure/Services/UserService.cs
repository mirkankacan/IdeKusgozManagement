using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userDTOs = new List<UserDTO>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var userDTO = user.Adapt<UserDTO>();
                    userDTO.Role = roles.FirstOrDefault();
                    userDTOs.Add(userDTO);
                }

                return ApiResponse<IEnumerable<UserDTO>>.Success(userDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllUsersAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<UserDTO>>.Error("Kullanıcılar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<UserDTO>> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.Error("Kullanıcı bulunamadı");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDTO = user.Adapt<UserDTO>();
                userDTO.Role = roles.FirstOrDefault();

                return ApiResponse<UserDTO>.Success(userDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserByIdAsync işleminde hata oluştu. UserId: {UserId}", id);
                return ApiResponse<UserDTO>.Error("Kullanıcı getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO)
        {
            try
            {
                // Username kontrolü
                var existingUser = await _userManager.FindByNameAsync(createUserDTO.UserName);
                if (existingUser != null)
                {
                    return ApiResponse<UserDTO>.Error("Bu kullanıcı adı zaten kullanılıyor");
                }

                var user = createUserDTO.Adapt<ApplicationUser>();

                var result = await _userManager.CreateAsync(user, createUserDTO.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<UserDTO>.Error(errors);
                }

                // Rol ata (eğer varsa)
                if (!string.IsNullOrEmpty(createUserDTO.RoleName))
                {
                    if (await _roleManager.RoleExistsAsync(createUserDTO.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, createUserDTO.RoleName);
                    }
                }

                var userDTO = user.Adapt<UserDTO>();
                userDTO.Role = createUserDTO.RoleName;

                return ApiResponse<UserDTO>.Success(userDTO, "Kullanıcı başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateUserAsync işleminde hata oluştu");
                return ApiResponse<UserDTO>.Error("Kullanıcı oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<UserDTO>> UpdateUserAsync(string id, UpdateUserDTO updateUserDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.Error("Kullanıcı bulunamadı");
                }

                // Username değişikliği kontrolü
                if (user.UserName != updateUserDTO.UserName)
                {
                    var existingUser = await _userManager.FindByNameAsync(updateUserDTO.UserName);
                    if (existingUser != null && existingUser.Id != id)
                    {
                        return ApiResponse<UserDTO>.Error("Bu kullanıcı adı zaten kullanılıyor");
                    }
                }

                updateUserDTO.Adapt(user);

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<UserDTO>.Error(errors);
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDTO = user.Adapt<UserDTO>();
                userDTO.Role = roles.FirstOrDefault();

                return ApiResponse<UserDTO>.Success(userDTO, "Kullanıcı başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUserAsync işleminde hata oluştu. UserId: {UserId}", id);
                return ApiResponse<UserDTO>.Error("Kullanıcı güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                // Soft delete
                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Kullanıcı başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteUserAsync işleminde hata oluştu. UserId: {UserId}", id);
                return ApiResponse<bool>.Error("Kullanıcı silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> AssignRoleToUserAsync(AssignRoleDTO assignRoleDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(assignRoleDTO.UserId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                if (!await _roleManager.RoleExistsAsync(assignRoleDTO.RoleName))
                {
                    return ApiResponse<bool>.Error("Rol bulunamadı");
                }

                // Kullanıcının mevcut rollerini temizle (çünkü sadece 1 rol olacak)
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                // Yeni rolü ata
                var result = await _userManager.AddToRoleAsync(user, assignRoleDTO.RoleName);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Rol başarıyla atandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AssignRoleToUserAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Rol atanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> ActivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Kullanıcı başarıyla aktifleştirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActivateUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<bool>.Error("Kullanıcı aktifleştirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeactivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Kullanıcı başarıyla pasifleştirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<bool>.Error("Kullanıcı pasifleştirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetUsersByRoleAsync(string roleName)
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
                _logger.LogError(ex, "GetUsersByRoleAsync işleminde hata oluştu. RoleName: {RoleName}", roleName);
                return ApiResponse<IEnumerable<UserDTO>>.Error("Role sahip kullanıcılar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Şifre başarıyla değiştirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChangePasswordAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<bool>.Error("Şifre değiştirilirken hata oluştu");
            }
        }
    }
}