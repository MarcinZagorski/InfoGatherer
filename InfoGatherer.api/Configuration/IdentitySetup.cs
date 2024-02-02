using InfoGatherer.api.Data;
using InfoGatherer.api.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using NLog.Common;

namespace InfoGatherer.api.Configuration
{
    public static class IdentitySetup
    {

        public static WebApplication SetupIdentity(this WebApplication app, string adminEmail, string adminPwd)
        {
            using var scope = app.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            foreach (var roleName in Enum.GetNames(typeof(AppRoles)))
            {
                if (!Task.Run(() => roleManager.RoleExistsAsync(roleName)).Result)
                {
                    Task.Run(() => roleManager.CreateAsync(new Role() { Name = roleName })).Wait();
                }
            }
            var adminRole = AppRoles.Admin.ToString();
            var adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "John",
                LastName = "Smith",
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var found = Task.Run(() => userManager.FindByNameAsync(adminUser.UserName)).Result;

            if (found == null)
            {
                Task.Run(() => userManager.CreateAsync(adminUser as AppUser, adminPwd)).Wait();


                found = Task.Run(() => userManager.FindByNameAsync(adminUser.UserName)).Result;

                Task.Run(() => userManager.AddToRolesAsync(found, [adminRole])).Wait();

            }
            return app;
        }
    }
    public enum AppRoles
    {
        Admin,
        User
    }
}
