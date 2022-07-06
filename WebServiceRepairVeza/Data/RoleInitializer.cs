using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace WebService.Data
{
    public static class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            string adminFirstName = "Владислав";
            string adminSecondName = "Попков";
            string adminMiddleName = "Эдуардович";
            string adminUserName = "admin1";
            string adminPassword = "admin_1";
            string adminTelephone = "+375292576155";
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = "admin" });
            }
            if (await roleManager.FindByNameAsync("employee") == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = "employee" });
            }
            if (await roleManager.FindByNameAsync("chief") == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = "chief" });
            }
            if (await userManager.FindByNameAsync(adminUserName) == null)
            {
                User admin = new()
                {
                    Name = adminFirstName,
                    SurName = adminSecondName,
                    MiddleName = adminMiddleName,
                    UserName = adminUserName,
                    PhoneNumber = adminTelephone
                };
                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                    await userManager.AddToRoleAsync(admin, "employee");
                    await userManager.AddToRoleAsync(admin, "chief");
                }
            }
        }
    }
}
