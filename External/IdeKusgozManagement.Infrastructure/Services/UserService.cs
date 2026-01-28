using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.UserDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ILogger<UserService> logger, IUnitOfWork unitOfWork) : IUserService
    {
        private readonly string[] superiorRoles = new[] { "Şef", "Yönetici", "Admin" };

        public async Task<ServiceResult<IEnumerable<UserDTO>>> GetUsersAsync(CancellationToken cancellationToken = default)
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
                userDTOs.OrderBy(x => x.Name);
                return ServiceResult<IEnumerable<UserDTO>>.SuccessAsOk(userDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUsersAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<UserDTO>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<UserDTO>.Error("Kullanıcı Bulunamadı", "Belirtilen ID'ye sahip kullanıcı bulunamadı.", HttpStatusCode.NotFound);
                }

                var roles = await userManager.GetRolesAsync(user);
                var userDTO = user.Adapt<UserDTO>();
                userDTO.RoleName = roles.FirstOrDefault();

                var superiorIds = await unitOfWork.GetRepository<IdtUserHierarchy>().Where(x => x.SubordinateId == userId).Select(x => x.SuperiorId).ToListAsync(cancellationToken);

                userDTO.SuperiorIds = superiorIds;

                return ServiceResult<UserDTO>.SuccessAsOk(userDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUserByIdAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<UserDTO>> CreateUserAsync(CreateUserDTO createUserDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                // Username kontrolü
                var existingUser = await userManager.FindByNameAsync(createUserDTO.TCNo);
                if (existingUser != null)
                {
                    return ServiceResult<UserDTO>.Error("Kullanıcı Adı Zaten Kullanılıyor", "Bu kullanıcı adı zaten kullanılıyor. Lütfen farklı bir TC No kullanın.", HttpStatusCode.BadRequest);
                }

                var user = createUserDTO.Adapt<ApplicationUser>();
                user.UserName = user.TCNo;
                user.Email = user.Email;
                user.HireDate = createUserDTO.HireDate ?? DateTime.Now;
                var result = await userManager.CreateAsync(user, createUserDTO.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<UserDTO>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
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

                var salaryAdvance = new IdtUserBalance
                {
                    UserId = user.Id,
                    Balance = createUserDTO.SalaryAdvanceBalance ?? 0,
                    Type = BalanceType.Salary,
                };

                var jobAdvance = new IdtUserBalance
                {
                    UserId = user.Id,
                    Balance = createUserDTO.JobAdvanceBalance ?? 0,
                    Type = BalanceType.Job,
                };
                await unitOfWork.GetRepository<IdtUserBalance>().AddAsync(salaryAdvance, cancellationToken);
                await unitOfWork.GetRepository<IdtUserBalance>().AddAsync(jobAdvance, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var userDTO = user.Adapt<UserDTO>();
                userDTO.RoleName = createUserDTO.RoleName;

                return ServiceResult<UserDTO>.SuccessAsCreated(userDTO, $"/api/users/{userDTO.Id}");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "CreateUserAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<UserDTO>> UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<UserDTO>.Error("Kullanıcı Bulunamadı", "Belirtilen ID'ye sahip kullanıcı bulunamadı.", HttpStatusCode.NotFound);
                }

                if (!string.IsNullOrEmpty(updateUserDTO.TCNo))
                {
                    var isUserExist = await userManager.Users.AnyAsync(x => x.TCNo == updateUserDTO.TCNo && x.UserName == updateUserDTO.TCNo && x.Id != userId, cancellationToken);
                    if (isUserExist)
                    {
                        return ServiceResult<UserDTO>.Error("TC No Zaten Kullanılıyor", "Bu TC No kullanılıyor. Lütfen farklı bir TC No kullanın.", HttpStatusCode.BadRequest);
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
                        var passwordErrors = passwordResult.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                        return ServiceResult<UserDTO>.ErrorFromValidation(passwordErrors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                    }
                }

                user.UserName = updateUserDTO.TCNo;
                user.Email = updateUserDTO.Email;
                user.TCNo = updateUserDTO.TCNo;
                user.Name = updateUserDTO.Name;
                user.Surname = updateUserDTO.Surname;
                user.IsActive = updateUserDTO.IsActive;
                user.DepartmentId = updateUserDTO.DepartmentId;
                user.DepartmentDutyId = updateUserDTO.DepartmentDutyId;

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<UserDTO>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
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
                        return ServiceResult<UserDTO>.Error("Geçersiz Üst Kullanıcı", $"Geçersiz üst kullanıcı ID: {superiorId}", HttpStatusCode.BadRequest);
                    }

                    if (superiorId == user.Id)
                    {
                        return ServiceResult<UserDTO>.Error("Hiyerarşi Hatası", "Kullanıcı kendi üstü olamaz.", HttpStatusCode.BadRequest);
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

                return ServiceResult<UserDTO>.SuccessAsOk(mappedUser);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "UpdateUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteUserAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<bool>.Error("Kullanıcı Bulunamadı", "Belirtilen ID'ye sahip kullanıcı bulunamadı.", HttpStatusCode.NotFound);
                }
                var userBalances = await unitOfWork.GetRepository<IdtUserBalance>().Where(x => x.UserId == userId).ToListAsync(cancellationToken);
                var userHierarchies = await unitOfWork.GetRepository<IdtUserHierarchy>().Where(x => x.SuperiorId == userId || x.SubordinateId == userId).ToListAsync();
                var userEntitlements = await unitOfWork.GetRepository<IdtAnnualLeaveEntitlement>().Where(x => x.UserId == userId).ToListAsync(cancellationToken);
                unitOfWork.GetRepository<IdtUserBalance>().RemoveRange(userBalances);
                unitOfWork.GetRepository<IdtUserHierarchy>().RemoveRange(userHierarchies);
                unitOfWork.GetRepository<IdtAnnualLeaveEntitlement>().RemoveRange(userEntitlements);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<bool>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                }
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "DeleteUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> AssignRoleToUserAsync(AssignRoleDTO assignRoleDTO)
        {
            try
            {
                var user = await userManager.FindByIdAsync(assignRoleDTO.UserId);
                if (user == null)
                {
                    return ServiceResult<bool>.Error("Kullanıcı Bulunamadı", "Belirtilen ID'ye sahip kullanıcı bulunamadı.", HttpStatusCode.NotFound);
                }

                if (!await roleManager.RoleExistsAsync(assignRoleDTO.RoleName))
                {
                    return ServiceResult<bool>.Error("Rol Bulunamadı", "Belirtilen role sahip rol bulunamadı.", HttpStatusCode.NotFound);
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
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<bool>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                }

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AssignRoleToUserAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> EnableUserAsync(string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<bool>.Error("Kullanıcı Bulunamadı", "Belirtilen ID'ye sahip kullanıcı bulunamadı.", HttpStatusCode.NotFound);
                }

                user.IsActive = true;
                var result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<bool>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                }

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DisableUserAsync(string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<bool>.Error("Kullanıcı Bulunamadı", "Belirtilen ID'ye sahip kullanıcı bulunamadı.", HttpStatusCode.NotFound);
                }

                user.IsActive = false;
                var result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<bool>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                }

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<bool>.Error("Kullanıcı Bulunamadı", "Belirtilen ID'ye sahip kullanıcı bulunamadı.", HttpStatusCode.NotFound);
                }

                var result = await userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select((e, i) => new { Index = i, Description = e.Description }).ToList();
                    return ServiceResult<bool>.ErrorFromValidation(errors.ToDictionary(e => e.Index.ToString(), e => (object)e.Description));
                }

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ChangePasswordAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<UserDTO>>> GetActiveSuperiorsAsync(CancellationToken cancellationToken = default)
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

                var mappedActiveSuperiors = superiorUsers.Adapt<IEnumerable<UserDTO>>().OrderBy(x => x.Name);

                return ServiceResult<IEnumerable<UserDTO>>.SuccessAsOk(mappedActiveSuperiors);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveSuperiorsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<UserDTO>>> GetSubordinatesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return ServiceResult<IEnumerable<UserDTO>>.Error("Validasyon Hatası", "Kullanıcı kimliği geçersiz.", HttpStatusCode.BadRequest);
                }

                var subordinateIds = await unitOfWork.GetRepository<IdtUserHierarchy>()
                    .Where(x => x.SuperiorId == userId)
                    .Select(x => x.SubordinateId)
                    .ToListAsync(cancellationToken);

                if (subordinateIds == null || !subordinateIds.Any())
                {
                    return ServiceResult<IEnumerable<UserDTO>>.SuccessAsOk(new List<UserDTO>());
                }
                var users = await userManager.Users.Where(x => subordinateIds.Contains(x.Id)).ToListAsync(cancellationToken);

                var mappedUsers = users.Adapt<IEnumerable<UserDTO>>().OrderBy(x => x.Name);

                return ServiceResult<IEnumerable<UserDTO>>.SuccessAsOk(mappedUsers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetSubordinatesByUserIdAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<AnnualLeaveBalanceDTO>> GetAnnualLeaveDaysByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return ServiceResult<AnnualLeaveBalanceDTO>.Error("Validasyon Hatası", "Kullanıcı ID'si boş geçilemez.", HttpStatusCode.BadRequest);

                var parameters = new object[] { userId };

                var funcResults = await unitOfWork.ExecuteTableValuedFunctionAsync<AnnualLeaveBalanceDTO>(
                        "dbo.IDF_AnnualLeaveBalance",
                        parameters,
                        cancellationToken);

                var result = funcResults.FirstOrDefault();

                return result != null
                    ? ServiceResult<AnnualLeaveBalanceDTO>.SuccessAsOk(result)
                    : ServiceResult<AnnualLeaveBalanceDTO>.Error("Veri Bulunamadı", "Yıllık izin bakiyesi verisi alınamadı.", HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAnnualLeaveDaysByUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<UserDTO>>> GetUsersByDepartmentDutyAsync(string departmentDutyId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(departmentDutyId))
                {
                    return ServiceResult<IEnumerable<UserDTO>>.Error("Validasyon Hatası", "Departman görev ID'si boş geçilemez.", HttpStatusCode.BadRequest);
                }

                var users = await userManager.Users.Where(x => x.DepartmentDutyId == departmentDutyId && x.IsActive == true).ToListAsync(cancellationToken);

                var mappedUsers = users.Adapt<IEnumerable<UserDTO>>().OrderBy(x => x.Name);

                return ServiceResult<IEnumerable<UserDTO>>.SuccessAsOk(mappedUsers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUsersByDepartmentDutyAsync işleminde hata oluştu. DepartmentDutyId: {DepartmentDutyId}", departmentDutyId);
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<UserDTO>>> GetUsersByDepartmentAsync(string departmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(departmentId))
                {
                    return ServiceResult<IEnumerable<UserDTO>>.Error("Validasyon Hatası", "Departman ID'si boş geçilemez.", HttpStatusCode.BadRequest);
                }

                var users = await userManager.Users.Where(x => x.DepartmentId == departmentId && x.IsActive == true).ToListAsync(cancellationToken);

                var mappedUsers = users.Adapt<IEnumerable<UserDTO>>().OrderBy(x => x.Name);

                return ServiceResult<IEnumerable<UserDTO>>.SuccessAsOk(mappedUsers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUsersByDepartmentAsync işleminde hata oluştu. DepartmentId: {DepartmentId}", departmentId);
                throw;
            }
        }
    }
}