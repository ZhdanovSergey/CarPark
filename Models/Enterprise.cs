using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace CarPark.Models
{
    public class Enterprise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public List<Driver> Drivers { get; set; } = new();
        public List<Vehicle> Vehicles { get; set; } = new();
        public List<DriverVehicle> DriversVehicles { get; set; } = new();
        public List<EnterpriseManager> EnterprisesManagers { get; set; } = new();
        public static async Task<IQueryable<Enterprise>> GetUserEnterprises
        (
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            ClaimsPrincipal claimsPrincipal
        )
        {
            if (claimsPrincipal.IsInRole(RoleNames.Admin))
                return dbContext.Enterprises;

            if (claimsPrincipal.IsInRole(RoleNames.Manager))
            {
                var managerId = (await userManager.FindByNameAsync(claimsPrincipal.Identity.Name))?.Id;

                return dbContext.Enterprises
                    .Include(e => e.EnterprisesManagers)
                    .Where(e => e.EnterprisesManagers.Any(em => em.ManagerId == managerId));
            }

            throw new Exception($"User role should be {RoleNames.Admin} or {RoleNames.Manager}");
        }
        public static async Task SaveAndBindWithManager
        (
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            ClaimsPrincipal claimsPrincipal,
            Enterprise enterprise
        )
        {
            dbContext.Add(enterprise);
            await dbContext.SaveChangesAsync();

            if (claimsPrincipal.IsInRole(RoleNames.Manager))
            {
                var manager = await userManager.FindByNameAsync(claimsPrincipal.Identity.Name);

                var savedEnterprise = await dbContext.Enterprises
                    .Where(e => e.Name == enterprise.Name && e.City == enterprise.City)
                    .FirstOrDefaultAsync();

                dbContext.Add(new EnterpriseManager
                {
                    EnterpriseId = savedEnterprise.Id,
                    ManagerId = manager.Id,
                });

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
