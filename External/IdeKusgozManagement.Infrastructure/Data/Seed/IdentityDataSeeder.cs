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
                    UserName = "1",
                    Email = "mirkankacan@ideaktif.com.tr",
                    IsActive = true,
                    TCNo = "1",
                    Name = "Admin",
                    Surname = "User",
                    IsExpatriate = true,
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
                    IsExpatriate = false
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
                    IsExpatriate = false
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
                    IsExpatriate = false
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