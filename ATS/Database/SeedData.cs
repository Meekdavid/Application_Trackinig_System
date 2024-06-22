using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using ATS.Models;
using ATS.Database;

namespace ATS.Database
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Create roles
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                EnsureRoleExists(roleManager, "Coordinator").Wait();

                // Look for any coordinators already in the database.
                if (!context.Users.Any(u => u.UserName == "coordinator@gmail.com"))
                {
                    var hasher = new PasswordHasher<ApplicationUser>();

                    var coordinator = new ApplicationUser
                    {
                        UserName = "coordinator@gmail.com",
                        NormalizedUserName = "COORDINATOR@GMAIL.COM",
                        Email = "coordinator@gmail.com",
                        NormalizedEmail = "COORDINATOR@GMAIL.COM",
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        Role = "Coordinator"
                    };

                    coordinator.PasswordHash = hasher.HashPassword(coordinator, "Meekdavid");

                    var result = userManager.CreateAsync(coordinator).Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(coordinator, "Coordinator").Wait();
                    }
                }
            }
        }

        private static async System.Threading.Tasks.Task EnsureRoleExists(RoleManager<IdentityRole> roleManager, string roleName)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
