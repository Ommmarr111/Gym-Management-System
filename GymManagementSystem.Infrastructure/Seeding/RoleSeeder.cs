using GymManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Infrastructure.Seeding
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            string[] roles = { "Admin", "Manager", "Receptionist" };
            // Create roles if they do not exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            // Seed Admin user
            await SeedAdminUser(userManager);
            await SeedManagerUser(userManager);
            await SeedReceptionistUser(userManager);
        }

        private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@gym.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }

        private static async Task SeedManagerUser(UserManager<ApplicationUser> userManager)
        {
            var managerEmail = "manager@gym.com";
            var managerUser = await userManager.FindByEmailAsync(managerEmail);

            if (managerUser == null)
            {
                var newManager = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    FullName = "Manager",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newManager, "Manager@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newManager, "Manager");
                }
            }
        }

        private static async Task SeedReceptionistUser(UserManager<ApplicationUser> userManager)
        {
            var receptionistEmail = "receptionist@gym.com";
            var receptionistUser = await userManager.FindByEmailAsync(receptionistEmail);

            if (receptionistUser == null)
            {
                var newReceptionist = new ApplicationUser
                {
                    UserName = receptionistEmail,
                    Email = receptionistEmail,
                    FullName = "Receptionist",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newReceptionist, "Receptionist@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newReceptionist, "Receptionist");
                }
            }
        }
    }
}