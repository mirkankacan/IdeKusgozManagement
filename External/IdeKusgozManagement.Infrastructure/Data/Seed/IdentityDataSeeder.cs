using IdeKusgozManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdeKusgozManagement.Infrastructure.Data.Seed
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedAdminUserAndRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            const string password = "123";
            const string adminRole = "Admin";
            const string chiefRole = "Şef";
            const string personnelRole = "Personel";
            const string unitManagerRole = "Yönetici";

            // Rolleri oluştur
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = adminRole, IsActive = true });
                await roleManager.CreateAsync(new ApplicationRole { Name = chiefRole, IsActive = true });
                await roleManager.CreateAsync(new ApplicationRole { Name = personnelRole, IsActive = true });
                await roleManager.CreateAsync(new ApplicationRole { Name = unitManagerRole, IsActive = true });
            }

            // İlk kullanıcı: Admin kullanıcısı
            var adminUser = await userManager.FindByNameAsync("1");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    Id = "f17def6f-ce42-42f7-b9f5-18865cc35a52",
                    UserName = "1",
                    Email = "mirkankacan@ideaktif.com.tr",
                    IsActive = true,
                    TCNo = "1",
                    Name = "Admin",
                    Surname = "User",
                    IsExpatriate = true,
                    HireDate = new DateTime(2019, 1, 1),
                    DepartmentId = "7E7F81B6-D74B-4760-A7D4-225A9D52BB66",
                    DepartmentDutyId = "3F60AC95-C89B-4C3D-81C8-42CAB98F82DD"
                };
                var result = await userManager.CreateAsync(adminUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception("Admin kullanıcısı oluşturulamadı: " + errors);
                }
            }
            var unitManagerUser = await userManager.FindByNameAsync("2");
            if (unitManagerUser == null)
            {
                unitManagerUser = new ApplicationUser
                {
                    UserName = "2",
                    Email = "mirkankacan@ideaktif.com.tr",
                    IsActive = true,
                    TCNo = "2",
                    Name = "Birim Yönetici",
                    Surname = "User",
                    IsExpatriate = false,
                    HireDate = new DateTime(2024, 1, 1),
                    DepartmentId = "110AFEFE-B02C-42AB-B070-1D0195A66D0D",
                    DepartmentDutyId = "7C69EEDE-86D4-4F59-808F-0B2262781391"
                };
                var result = await userManager.CreateAsync(unitManagerUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(unitManagerUser, unitManagerRole);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception("Birim yönetici kullanıcısı oluşturulamadı: " + errors);
                }
            }
            var chiefUser = await userManager.FindByNameAsync("3");
            if (chiefUser == null)
            {
                chiefUser = new ApplicationUser
                {
                    UserName = "3",
                    Email = "mirkankacan@ideaktif.com.tr",
                    IsActive = true,
                    TCNo = "3",
                    Name = "Şef",
                    Surname = "User",
                    IsExpatriate = false,
                    HireDate = new DateTime(2024, 1, 1),
                    DepartmentId = "3916A3F1-8B9B-4DFC-A236-C9213D12DF1C",
                    DepartmentDutyId = "E6DD7A93-FB89-4FD3-847B-20EAF7F83A77"
                };
                var result = await userManager.CreateAsync(chiefUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(chiefUser, chiefRole);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception("Şef kullanıcısı oluşturulamadı: " + errors);
                }
            }
            var personnelUser = await userManager.FindByNameAsync("4");
            if (personnelUser == null)
            {
                personnelUser = new ApplicationUser
                {
                    UserName = "4",
                    Email = "mirkankacan@ideaktif.com.tr",
                    IsActive = true,
                    TCNo = "4",
                    Name = "Personel",
                    Surname = "User",
                    IsExpatriate = false,
                    HireDate = new DateTime(2024, 1, 1),
                    DepartmentId = "5D6DE7B9-F6E7-4224-A78C-6DA664C641F0",
                    DepartmentDutyId = "262CA528-8EF1-4DD3-823E-83D35CB96292"
                };
                var result = await userManager.CreateAsync(personnelUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(personnelUser, personnelRole);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception("Personel kullanıcısı oluşturulamadı: " + errors);
                }
            }
        }
    }
}