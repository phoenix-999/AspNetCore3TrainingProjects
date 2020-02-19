using IdentityAppFromEmpty.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAppFromEmpty
{
    public class StartInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminName = "Admin";
            string password = "_Aa123456";
            string adminRoleName = "Admins";
            string userRoleName = "Users";

            if (await roleManager.FindByNameAsync(adminRoleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            if (await roleManager.FindByNameAsync(userRoleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(userRoleName));
            }

            if (await userManager.FindByNameAsync(adminName) == null)
            {
                User admin = new User { UserName = adminName };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, adminRoleName);
                }
            }
        }
    }
}
