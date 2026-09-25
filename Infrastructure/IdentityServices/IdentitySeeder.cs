using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IdentityServices;

public class IdentitySeeder ()
{


    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = [
        "Admin",
        "User"
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                 await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}