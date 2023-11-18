using CarPark.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace CarPark.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public int? ActiveDriverId { get; set; }
        public int EnterpriseId { get; set; }
        public string RegistrationNumber { get; set; }
        public int Mileage { get; set; }
        public int Price { get; set; }
        public int Year { get; set; }
        public Brand? Brand { get; set; }
        public Driver? ActiveDriver { get; set; }
        public Enterprise? Enterprise { get; set; }
        public List<DriverVehicle> DriversVehicles { get; set; } = new();
        public Vehicle() { }
        public Vehicle(VehicleViewModel vehicleVM)
        {
            Id = vehicleVM.Id;
            ActiveDriverId = vehicleVM.ActiveDriverId;
            BrandId = vehicleVM.BrandId;
            EnterpriseId = vehicleVM.EnterpriseId;
            RegistrationNumber = vehicleVM.RegistrationNumber;
            Mileage = vehicleVM.Mileage;
            Price = vehicleVM.Price;
            Year = vehicleVM.Year;
        }
        public static IQueryable<Vehicle> GetUserVehicles
        (
            ApplicationDbContext dbContext,
            ClaimsPrincipal claimsPrincipal,
            int userId
        )
        {
            if (claimsPrincipal.IsInRole(RoleNames.Admin))
                return dbContext.Vehicles;

            if (claimsPrincipal.IsInRole(RoleNames.Manager))
            {
                return dbContext.Vehicles
                    .Include(v => v.Enterprise)
                        .ThenInclude(e => e.EnterprisesManagers)
                    .Where(v => v.Enterprise.EnterprisesManagers.Any(em => em.ManagerId == userId));
            }

            throw new Exception($"User role should be {RoleNames.Admin} or {RoleNames.Manager}");
        }
        public static async Task RemoveActiveDriver(ApplicationDbContext dbContext, int? activeDriverId)
        {
            var prevActiveVehicle = await dbContext.Vehicles
                .FirstOrDefaultAsync(v => v.ActiveDriverId == activeDriverId);

            if (prevActiveVehicle is not null)
            {
                prevActiveVehicle.ActiveDriverId = null;
                dbContext.Update(prevActiveVehicle);
            }
        }
    }
}
