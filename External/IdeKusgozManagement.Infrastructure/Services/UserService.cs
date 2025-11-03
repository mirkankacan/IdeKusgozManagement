using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ILogger<UserService> logger, IUnitOfWork unitOfWork) : IUserService
    {
        private readonly string[] superiorRoles = new[] { "Şef", "Yönetici", "Admin" };

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await userManager.Users.ToListAsync(cancellationToken);

                // Tüm kullanıcı ID'lerini topla
                var userIds = users.Select(u => u.Id).ToList();

                // Tüm hiyerarşileri tek sorguda getir
                var hierarchies = await unitOfWork.GetRepository<IdtUserHierarchy>()
                    .Where(x => userIds.Contains(x.SubordinateId))
                    .ToListAsync(cancellationToken);

                // Hiyerarşileri kullanıcı ID'sine göre grupla
                var hierarchyLookup = hierarchies
                    .GroupBy(h => h.SubordinateId)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.SuperiorId).ToList());

                // Tüm kullanıcı rollerini tek seferde al (Identity'de böyle bir metod varsa)
                var userDTOs = new List<UserDTO>();

                foreach (var user in users)
                {
                    var userDTO = user.Adapt<UserDTO>();

                    // Lookup'tan al, yoksa boş liste
                    userDTO.SuperiorIds = hierarchyLookup.TryGetValue(user.Id, out var superiors)
                        ? superiors
                        : new List<string>();

                    var roles = await userManager.GetRolesAsync(user);
                    userDTO.RoleName = roles.FirstOrDefault();

                    userDTOs.Add(userDTO);
                }

                return ApiResponse<IEnumerable<UserDTO>>.Success(userDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUsersAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<UserDTO>>.Error("Kullanıcılar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<UserDTO>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.Error("Kullanıcı bulunamadı");
                }

                var roles = await userManager.GetRolesAsync(user);
                var userDTO = user.Adapt<UserDTO>();
                userDTO.RoleName = roles.FirstOrDefault();

                var superiorIds = await unitOfWork.GetRepository<IdtUserHierarchy>().Where(x => x.SubordinateId == userId).Select(x => x.SuperiorId).ToListAsync(cancellationToken);

                userDTO.SuperiorIds = superiorIds;

                return ApiResponse<UserDTO>.Success(userDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUserByIdAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<UserDTO>.Error("Kullanıcı getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                // Username kontrolü
                var existingUser = await userManager.FindByNameAsync(createUserDTO.TCNo);
                if (existingUser != null)
                {
                    return ApiResponse<UserDTO>.Error("Bu kullanıcı adı zaten kullanılıyor");
                }

                var user = createUserDTO.Adapt<ApplicationUser>();
                user.UserName = user.TCNo;
                user.Email = user.Email;
                user.HireDate = createUserDTO.HireDate ?? DateTime.Now;
                var result = await userManager.CreateAsync(user, createUserDTO.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<UserDTO>.Error(errors);
                }

                if (!string.IsNullOrEmpty(createUserDTO.RoleName))
                {
                    if (await roleManager.RoleExistsAsync(createUserDTO.RoleName))
                    {
                        await userManager.AddToRoleAsync(user, createUserDTO.RoleName);
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
                        await unitOfWork.GetRepository<IdtUserHierarchy>().AddRangeAsync(hierarchyList, cancellationToken);
                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var userDTO = user.Adapt<UserDTO>();
                userDTO.RoleName = createUserDTO.RoleName;

                return ApiResponse<UserDTO>.Success(userDTO, "Kullanıcı başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "CreateUserAsync işleminde hata oluştu");
                return ApiResponse<UserDTO>.Error("Kullanıcı oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<UserDTO>> UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<UserDTO>.Error("Kullanıcı bulunamadı");
                }

                if (!string.IsNullOrEmpty(updateUserDTO.TCNo))
                {
                    var isUserExist = await userManager.Users.AnyAsync(x => x.TCNo == updateUserDTO.TCNo && x.UserName == updateUserDTO.TCNo && x.Id != userId, cancellationToken);
                    if (isUserExist)
                    {
                        return ApiResponse<UserDTO>.Error("Bu TC No kullanılıyor");
                    }
                }

                if (!string.IsNullOrEmpty(updateUserDTO.RoleName))
                {
                    var currentRoles = await userManager.GetRolesAsync(user);
                    if (currentRoles.Any())
                    {
                        await userManager.RemoveFromRolesAsync(user, currentRoles);
                    }

                    if (await roleManager.RoleExistsAsync(updateUserDTO.RoleName))
                    {
                        await userManager.AddToRoleAsync(user, updateUserDTO.RoleName);
                    }
                }

                // Şifre güncelleme kontrolü
                if (!string.IsNullOrEmpty(updateUserDTO.Password) && updateUserDTO.Password.Length > 3)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await userManager.ResetPasswordAsync(user, token, updateUserDTO.Password);
                    if (!passwordResult.Succeeded)
                    {
                        var passwordErrors = passwordResult.Errors.Select(e => e.Description).ToList();
                        return ApiResponse<UserDTO>.Error(passwordErrors);
                    }
                }

                user.UserName = updateUserDTO.TCNo;
                user.Email = updateUserDTO.Email;
                user.TCNo = updateUserDTO.TCNo;
                user.Name = updateUserDTO.Name;
                user.Surname = updateUserDTO.Surname;
                user.IsActive = updateUserDTO.IsActive;

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<UserDTO>.Error(errors);
                }

                var superiors = await unitOfWork.GetRepository<IdtUserHierarchy>().Where(x => x.SubordinateId == user.Id).ToListAsync(cancellationToken);
                if (superiors.Any())
                {
                    unitOfWork.GetRepository<IdtUserHierarchy>().RemoveRange(superiors);
                }
                var newHierarchies = new List<IdtUserHierarchy>();
                foreach (var superiorId in updateUserDTO.SuperiorIds.Where(id => !string.IsNullOrEmpty(id)))
                {
                    // Superior'un gerçekten var olduğunu kontrol et
                    var superiorExists = await userManager.FindByIdAsync(superiorId);
                    if (superiorExists == null || !superiorExists.IsActive)
                    {
                        return ApiResponse<UserDTO>.Error($"Geçersiz üst kullanıcı ID: {superiorId}");
                    }

                    if (superiorId == user.Id)
                    {
                        return ApiResponse<UserDTO>.Error("Kullanıcı kendi üstü olamaz");
                    }

                    var hierarchy = new IdtUserHierarchy
                    {
                        SubordinateId = user.Id,
                        SuperiorId = superiorId,
                    };
                    newHierarchies.Add(hierarchy);
                }

                if (newHierarchies.Any())
                {
                    await unitOfWork.GetRepository<IdtUserHierarchy>().AddRangeAsync(newHierarchies, cancellationToken);
                }
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var mappedUser = user.Adapt<UserDTO>();
                var role = (await userManager.GetRolesAsync(user)).FirstOrDefault()!;
                mappedUser.RoleName = role;

                return ApiResponse<UserDTO>.Success(mappedUser, "Kullanıcı başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "UpdateUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<UserDTO>.Error("Kullanıcı güncellenirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Kullanıcı başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<bool>.Error("Kullanıcı silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> AssignRoleToUserAsync(AssignRoleDTO assignRoleDTO)
        {
            try
            {
                var user = await userManager.FindByIdAsync(assignRoleDTO.UserId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                if (!await roleManager.RoleExistsAsync(assignRoleDTO.RoleName))
                {
                    return ApiResponse<bool>.Error("Rol bulunamadı");
                }

                // Kullanıcının mevcut rollerini temizle (çünkü sadece 1 rol olacak)
                var currentRoles = await userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                // Yeni rolü ata
                var result = await userManager.AddToRoleAsync(user, assignRoleDTO.RoleName);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Rol başarıyla atandı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AssignRoleToUserAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Rol atanırken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> EnableUserAsync(string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                user.IsActive = true;
                var result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Kullanıcı başarıyla aktifleştirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<bool>.Error("Kullanıcı aktifleştirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DisableUserAsync(string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                user.IsActive = false;
                var result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Kullanıcı başarıyla pasifleştirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<bool>.Error("Kullanıcı pasifleştirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error("Kullanıcı bulunamadı");
                }

                var result = await userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.Error(errors);
                }

                return ApiResponse<bool>.Success(true, "Şifre başarıyla değiştirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ChangePasswordAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<bool>.Error("Şifre değiştirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetActiveSuperiorsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Aktif kullanıcıları getir
                var users = await userManager.Users
                    .Where(u => u.IsActive)
                    .ToListAsync(cancellationToken);

                // Üst düzey rollere sahip kullanıcıları filtrele
                var superiorUsers = new List<ApplicationUser>();
                foreach (var user in users)
                {
                    var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                    if (userRole != null && superiorRoles.Contains(userRole))
                    {
                        superiorUsers.Add(user);
                    }
                }

                var mappedActiveSuperiors = superiorUsers.Adapt<IEnumerable<UserDTO>>();

                return ApiResponse<IEnumerable<UserDTO>>.Success(mappedActiveSuperiors);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveSuperiorsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<UserDTO>>.Error("Aktif yöneticiler getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetSubordinatesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<IEnumerable<UserDTO>>.Error("Kullanıcı kimliği geçersiz");
                }

                var subordinateIds = await unitOfWork.GetRepository<IdtUserHierarchy>()
                    .Where(x => x.SuperiorId == userId)
                    .Select(x => x.SubordinateId)
                    .ToListAsync(cancellationToken);

                if (subordinateIds == null || !subordinateIds.Any())
                {
                    return ApiResponse<IEnumerable<UserDTO>>.Success(new List<UserDTO>(), "Alt kullanıcılar bulunamadı");
                }
                var users = await userManager.Users.Where(x => subordinateIds.Contains(x.Id)).ToListAsync(cancellationToken);

                var mappedUsers = users.Adapt<IEnumerable<UserDTO>>();

                return ApiResponse<IEnumerable<UserDTO>>.Success(mappedUsers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetSubordinatesByUserId işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<IEnumerable<UserDTO>>.Error("Atanmış kullanıcılar getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<AnnualLeaveBalanceDTO>> GetAnnualLeaveDaysByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return ApiResponse<AnnualLeaveBalanceDTO>.Error("Kullanıcı ID'si boş geçilemez");

                var parameters = new object[] { userId };

                var funcResults = await unitOfWork.ExecuteTableValuedFunctionAsync<AnnualLeaveBalanceDTO>(
                        "dbo.IDF_AnnualLeaveBalance",
                        parameters,
                        cancellationToken);

                var result = funcResults.FirstOrDefault();

                return result != null
                    ? ApiResponse<AnnualLeaveBalanceDTO>.Success(result)
                    : ApiResponse<AnnualLeaveBalanceDTO>.Error("Veri alınamadı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAnnualLeaveDaysAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<AnnualLeaveBalanceDTO>.Error("Kullanıcının yıllık izin verileri alınırken hata oluştu");
            }
        }
    }
}