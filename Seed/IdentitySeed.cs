using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentApp.Models;

namespace RecruitmentApp.Seed
{
    public static class IdentitySeed
    {
        public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {


            // Define roles
            string[] roleNames = { "Admin", "User", "Recruiter" };

            // Create roles if they do not exist
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create an admin user if it does not exist
            var adminEmail = "chrisnguyeen2000@gmail.com";
            var adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Avatar = "/images/icons/avatar-default.svg"
                };

                var createAdmin = await userManager.CreateAsync(newAdmin, adminPassword);
                if (createAdmin.Succeeded)
                {
                    // Assign Admin role to the user
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

            // Seed 10 random users for each role
            var random = new Random();

            foreach (var roleName in roleNames)
            {
                for (int i = 0; i < 10; i++)
                {
                    var userEmail = $"{roleName.ToLower()}user{i + 1}@example.com";
                    var password = "Password@123"; // Default password for all users
                    var user = await userManager.FindByEmailAsync(userEmail);

                    if (user == null)
                    {
                        var newUser = new AppUser
                        {
                            UserName = userEmail,
                            Email = userEmail,
                            EmailConfirmed = true,
                            Avatar = "/images/icons/avatar-default.svg"
                        };

                        var createUser = await userManager.CreateAsync(newUser, password);
                        if (createUser.Succeeded)
                        {
                            // Assign role to the user
                            await userManager.AddToRoleAsync(newUser, roleName);
                        }
                    }
                }
            }
        }
    }
}