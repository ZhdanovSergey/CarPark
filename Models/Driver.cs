using CarPark.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarPark.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public int EnterpriseId { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        public Vehicle? ActiveVehicle { get; set; }
        public Enterprise? Enterprise { get; set; }
        public List<DriverVehicle> DriversVehicles { get; set; } = new();
        public Driver() { }
        public Driver(DriverViewModel driverVM)
        {
            Id = driverVM.Id;
            Name = driverVM.Name;
            Salary = driverVM.Salary;
            EnterpriseId = driverVM.EnterpriseId;
        }
        public static IQueryable<Driver> GetUserDrivers
        (
            ApplicationDbContext dbContext,
            ClaimsPrincipal claimsPrincipal,
            int userId
        )
        {
            if (claimsPrincipal.IsInRole(RoleNames.Admin))
                return dbContext.Drivers;

            if (claimsPrincipal.IsInRole(RoleNames.Manager))
            {
                return dbContext.Drivers
                    .Include(d => d.Enterprise)
                        .ThenInclude(e => e.EnterprisesManagers)
                    .Where(d => d.Enterprise.EnterprisesManagers.Any(em => em.ManagerId == userId));
            }

            throw new Exception($"User role should be {RoleNames.Admin} or {RoleNames.Manager}");
        }
    }
}
