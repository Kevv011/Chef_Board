using Chef_Board_Desafio3.Models;
using Microsoft.AspNetCore.Identity;

namespace Chef_Board_Desafio3.Data;

public static class IdentityDataSeeder
{
    public const string AdministratorRole = "Administrator";
    public const string UserRole = "User";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var roleName in new[] { AdministratorRole, UserRole })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        await CreateUserIfMissingAsync(userManager, "admin@chefboard.local", "Admin123!", AdministratorRole);
        await CreateUserIfMissingAsync(userManager, "user@chefboard.local", "User123!", UserRole);
    }

    private static async Task CreateUserIfMissingAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Could not create the seeded user '{email}': {string.Join(", ", result.Errors.Select(error => error.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
