using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ChurchAttendance.API.Seed;

/// <summary>
/// Makes sure the "User" and "Admin" roles exist and, when no admin exists yet, creates the first one
/// from the SeedAdmin configuration section (PhoneNumber, Password, PersonName).
/// Put those values in user-secrets or environment variables, not in appsettings.json.
/// </summary>
public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
    {
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var userRole = UserTypeOptions.User.ToString();
        var adminRole = UserTypeOptions.Admin.ToString();

        foreach (var role in new[] { userRole, adminRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
        }

        var section = configuration.GetSection("SeedAdmin");
        var phone = section["PhoneNumber"];
        var password = section["Password"];
        var name = section["PersonName"] ?? "Administrator";

        if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogInformation("SeedAdmin is not configured; skipping initial admin creation.");
            return;
        }

        if ((await userManager.GetUsersInRoleAsync(adminRole)).Count > 0)
            return;

        var user = await userManager.FindByNameAsync(phone);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = phone,
                PhoneNumber = phone,
                PersonName = name,
                isActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await userManager.CreateAsync(user, password);
            if (!created.Succeeded)
            {
                logger.LogError("Could not create the seed admin: {Errors}",
                    string.Join(" | ", created.Errors.Select(e => e.Description)));
                return;
            }
        }

        foreach (var role in new[] { userRole, adminRole })
        {
            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
        }

        logger.LogInformation("Seed admin ready.");
    }
}
