using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameQuest.Models;

public static class IdentitySeed
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var gqDb = scope.ServiceProvider.GetRequiredService<GameQuestDbContext>();

        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@gamequest.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Admin user oluşturulamadı: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Domain Users tablosu (int key) ile Identity kullanıcılarını eşleştiriyoruz.
        var adminIdentityId = await userManager.GetUserIdAsync(adminUser);
        var domainAdmin = await gqDb.Users.FirstOrDefaultAsync(u => u.IdentityUserId == adminIdentityId);
        if (domainAdmin is null)
        {
            domainAdmin = new User
            {
                IdentityUserId = adminIdentityId,
                Username = adminUser.UserName ?? adminEmail,
                Email = adminUser.Email ?? adminEmail,
                IsActive = true
            };

            gqDb.Users.Add(domainAdmin);
            await gqDb.SaveChangesAsync();
        }
    }
}
