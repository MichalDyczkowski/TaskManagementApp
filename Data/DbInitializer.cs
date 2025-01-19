using Microsoft.AspNetCore.Identity;
using TaskManagementApp.Models;

namespace TaskManagementApp.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync("Administrator"))
                await roleManager.CreateAsync(new IdentityRole("Administrator"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            var adminEmail = "admin@example.com";
            var adminPass = "Admin123!";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var res = await userManager.CreateAsync(adminUser, adminPass);
                if (res.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }
        }
    }
}
