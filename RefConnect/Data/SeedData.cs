using Microsoft.AspNetCore.Identity;
using RefConnect.Models;

namespace RefConnect.Data;

public static class SeedData
{
    public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var adminRole = configuration["Seed:AdminRole"] ?? "Admin";
        var adminUserName = configuration["Seed:AdminUser:UserName"] ?? "admin";
        var adminEmail = configuration["Seed:AdminUser:Email"] ?? "admin@refconnect.local";
        var adminPassword = configuration["Seed:AdminUser:Password"] ?? "Admin123!";

        // 1) Ensure role exists
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new InvalidOperationException($"Seed failed creating role '{adminRole}': {errors}");
            }
        }

        // 2) Ensure user exists
        var user = await userManager.FindByNameAsync(adminUserName);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = configuration["Seed:AdminUser:FirstName"] ?? "System",
                LastName = configuration["Seed:AdminUser:LastName"] ?? "Admin",
                Description = configuration["Seed:AdminUser:Description"] ?? "Seeded admin user",
                ProfileImageUrl = configuration["Seed:AdminUser:ProfileImageUrl"],
                IsProfilePublic = false,
            };

            var createResult = await userManager.CreateAsync(user, adminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new InvalidOperationException($"Seed failed creating admin user '{adminUserName}': {errors}");
            }
        }

        // 3) Ensure user is in role
        if (!await userManager.IsInRoleAsync(user, adminRole))
        {
            var addResult = await userManager.AddToRoleAsync(user, adminRole);
            if (!addResult.Succeeded)
            {
                var errors = string.Join("; ", addResult.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new InvalidOperationException($"Seed failed adding user '{adminUserName}' to role '{adminRole}': {errors}");
            }
        }
    }
}
