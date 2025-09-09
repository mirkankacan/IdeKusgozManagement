using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserDTOs;
using IdeKusgozManagement.Application.Interfaces.Repositories;
using IdeKusgozManagement.Application.Interfaces.Services;
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
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UserService> logger,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _userManager.Users.ToListAsync(cancellationToken);

                var userDTOs = new List<UserDTO>();

                foreach (var user in users)
                {
                    var userDTO = user.Adapt<UserDTO>();
                    var superiorIds = await _unitOfWork.Repository<IdtUserHierarchy>()
                  .SelectNoTrackingAsync(
                      selector: x => x.SuperiorId,
                      predicate: x => x.SubordinateId == user.Id, cancellationToken
                  );
                    var roles = await _userManager.GetRolesAsync(user);

                    userDTO.SuperiorIds = superiorIds.ToList();
                    userDTO.RoleName = roles.FirstOrDefault();
                    userDTOs.Add(userDTO);
                }

                return ApiResponse<IEnumerable<UserDTO>>.Success(userDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUsersAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<UserDTO>>.Error("Kullanıcılar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<UserDTO>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.Error("Kullanıcı bulunamadı");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDTO = user.Adapt<UserDTO>();
                userDTO.RoleName = roles.FirstOrDefault();

                var superiorIds = await _unitOfWork.Repository<IdtUserHierarchy>()
                    .SelectNoTrackingAsync(
                        selector: x => x.SuperiorId,
                        predicate: x => x.SubordinateId == userId, cancellationToken
                    );

                userDTO.SuperiorIds = superiorIds.ToList();

                return ApiResponse<UserDTO>.Success(userDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserByIdAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<UserDTO>.Error("Kullanıcı getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

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

                if (!string.IsNullOrEmpty(createUserDTO.RoleName))
                {
                    if (await _roleManager.RoleExistsAsync(createUserDTO.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, createUserDTO.RoleName);
                    }
                }

                // Birden fazla superior için hierarchy kayıtları oluştur
                if (createUserDTO.SuperiorIds != null && createUserDTO.SuperiorIds.Any())
                {
                    var hierarchyList = new List<IdtUserHierarchy>();

                    foreach (var superiorId in createUserDTO.SuperiorIds)
                    {
                        if (!string.IsNullOrEmpty(superiorId))
                        {
                            var userHierarchy = new IdtUserHierarchy
                            {
                                SubordinateId = user.Id,
                                SuperiorId = superiorId,
                            };
                            hierarchyList.Add(userHierarchy);
                        }
                    }

                    if (hierarchyList.Any())
                    {
                        await _unitOfWork.Repository<IdtUserHierarchy>().AddRangeAsync(hierarchyList);
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                var userDTO = user.Adapt<UserDTO>();
                userDTO.RoleName = createUserDTO.RoleName;

                return ApiResponse<UserDTO>.Success(userDTO, "Kullanıcı başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "CreateUserAsync işleminde hata oluştu");
                return ApiResponse<UserDTO>.Error("Kullanıcı oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<UserDTO>> UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.Error("Kullanıcı bulunamadı");
                }

                // Username değişikliği kontrolü
                if (user.UserName != updateUserDTO.UserName)
                {
                    var existingUser = await _userManager.FindByNameAsync(updateUserDTO.UserName);
                    if (existingUser != null && existingUser.Id != userId)
                    {
                        return ApiResponse<UserDTO>.Error("Bu kullanıcı adı kullanılıyor");
                    }
                }

                if (!string.IsNullOrEmpty(updateUserDTO.RoleName))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (currentRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    }

                    if (await _roleManager.RoleExistsAsync(updateUserDTO.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, updateUserDTO.RoleName);
                    }
                }

                // Şifre güncelleme kontrolü
                if (!string.IsNullOrEmpty(updateUserDTO.Password) && updateUserDTO.Password.Length > 3)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, updateUserDTO.Password);
                    if (!passwordResult.Succeeded)
                    {
                        var passwordErrors = passwordResult.Errors.Select(e => e.Description).ToList();
                        return ApiResponse<UserDTO>.Error(passwordErrors);
                    }
                }

                user.UserName = updateUserDTO.UserName;
                user.Name = updateUserDTO.Name;
                user.Surname = updateUserDTO.Surname;
                user.IsActive = updateUserDTO.IsActive.HasValue ? updateUserDTO.IsActive.Value : user.IsActive;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<UserDTO>.Error(errors);
                }

                var deleteResult = await _unitOfWork.Repository<IdtUserHierarchy>().DeleteRangeAsync(h => h.SubordinateId == user.Id, cancellationToken);

                // Yeni hierarchy kayıtları oluştur
                if (updateUserDTO.SuperiorIds != null && updateUserDTO.SuperiorIds.Any())
                {
                    var newHierarchies = new List<IdtUserHierarchy>();

                    foreach (var superiorId in updateUserDTO.SuperiorIds.Where(id => !string.IsNullOrEmpty(id)))
                    {
                        var hierarchy = new IdtUserHierarchy
                        {
                            SubordinateId = user.Id,
                            SuperiorId = superiorId,
                        };
                        newHierarchies.Add(hierarchy);
                    }

                    if (newHierarchies.Any())
                    {
                        await _unitOfWork.Repository<IdtUserHierarchy>().AddRangeAsync(newHierarchies);
                    }
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                var roles = await _userManager.GetRolesAsync(user);
                var userDTO = user.Adapt<UserDTO>();
                userDTO.RoleName = roles.FirstOrDefault();

                return ApiResponse<UserDTO>.Success(userDTO, "Kullanıcı başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "UpdateUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<UserDTO>.Error("Kullanıcı güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Kullanıcı başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
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

        public async Task<ApiResponse<bool>> EnableUserAsync(string userId)
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
                _logger.LogError(ex, "EnableUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<bool>.Error("Kullanıcı aktifleştirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DisableUserAsync(string userId)
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
                _logger.LogError(ex, "DisableUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<bool>.Error("Kullanıcı pasifleştirilirken hata oluştu");
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

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetActiveSuperiorsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var targetRoles = new[] { "Şef", "Yönetici", "Admin" };
                var userDTOs = new List<UserDTO>();

                foreach (var role in targetRoles)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role);

                    foreach (var user in usersInRole.Where(x => x.IsActive == true))
                    {
                        var userDTO = user.Adapt<UserDTO>();
                        userDTO.RoleName = role;
                        userDTO.SuperiorIds = new();
                        userDTOs.Add(userDTO);
                    }
                }

                return ApiResponse<IEnumerable<UserDTO>>.Success(userDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveSuperiorsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<UserDTO>>.Error("Kullanıcılar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetSubordinatesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var subordinateIds = await _unitOfWork.Repository<IdtUserHierarchy>()
                    .SelectNoTrackingAsync(
                        selector: x => x.SubordinateId,
                        predicate: x => x.SuperiorId == userId,
                        cancellationToken
                    );

                // Eğer hiç astı yoksa boş liste döndür
                if (!subordinateIds.Any())
                {
                    return ApiResponse<IEnumerable<UserDTO>>.Success(new List<UserDTO>());
                }

                var users = await _userManager.Users
                    .Where(u => subordinateIds.Contains(u.Id))
                    .ToListAsync(cancellationToken);

                var userDTOs = new List<UserDTO>();

                foreach (var user in users)
                {
                    var userDTO = user.Adapt<UserDTO>();

                    var superiorIds = await _unitOfWork.Repository<IdtUserHierarchy>()
                        .SelectNoTrackingAsync(
                            selector: x => x.SuperiorId,
                            predicate: x => x.SubordinateId == user.Id,
                            cancellationToken
                        );

                    var roles = await _userManager.GetRolesAsync(user);
                    userDTO.SuperiorIds = superiorIds.ToList();
                    userDTO.RoleName = roles.FirstOrDefault();
                    userDTOs.Add(userDTO);
                }

                return ApiResponse<IEnumerable<UserDTO>>.Success(userDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSubordinatesByUserId işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<IEnumerable<UserDTO>>.Error("Atanmış kullanıcılar getirilirken hata oluştu");
            }
        }
    }
}