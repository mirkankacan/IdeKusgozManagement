using IdeKusgozManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdeKusgozManagement.Infrastructure.Data.Seed
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedAdminUserAndRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            const string adminEmail = "ideaktif";
            const string adminPassword = "!!idea2019..";

            const string adminRole = "Admin";
            const string chiefRole = "Şef";
            const string personnelRole = "Personel";

            // Rolleri oluştur
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = adminRole });
                await roleManager.CreateAsync(new ApplicationRole { Name = chiefRole });
                await roleManager.CreateAsync(new ApplicationRole { Name = personnelRole });
            }

            // İlk kullanıcı: Admin kullanıcısı
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    IsActive = true,
                    Name = "İdeaktif",
                    Surname = "Yazılım"
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
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
        }
    }
}