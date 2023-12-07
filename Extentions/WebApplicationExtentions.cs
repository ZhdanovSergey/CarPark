using CarPark.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace CarPark.Extentions
{
    public static class WebApplicationExtentions
    {
        public static WebApplication UseSeedData(this WebApplication builder)
        {
            const string ADMIN_USER_NAME = "admin";
            const string ADMIN_USER_PASSWORD = "Admin1!";

            using var scope = builder.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (dbContext.Roles.Count() == 0)
            {
                roleManager.CreateAsync(new ApplicationRole { Name = RoleNames.Admin }).Wait();
                roleManager.CreateAsync(new ApplicationRole { Name = RoleNames.Manager }).Wait();
            }

            if (!dbContext.Users.Any(x => x.UserName == ADMIN_USER_NAME))
            {
                var user = new ApplicationUser { UserName = ADMIN_USER_NAME };
                userManager.CreateAsync(user, ADMIN_USER_PASSWORD).Wait();
                userManager.AddToRoleAsync(user, RoleNames.Admin).Wait();
            }

            return builder;
        }
    }
}