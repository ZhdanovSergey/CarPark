using CarPark.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarPark.ViewModels
{
    public class DriverViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        public int EnterpriseId { get; set; }
        public List<int> VehiclesIds { get; set; } = new();
        public int? ActiveVehicleId { get; set; }
        public SelectList? EnterprisesSelectList { get; set; }
        public MultiSelectList? VehiclesSelectList { get; set; }
        public SelectList? ActiveVehicleSelectList { get; set; }
        public DriverViewModel() { }
        public DriverViewModel(Driver driver)
        {
            Id = driver.Id;
            Name = driver.Name;
            Salary = driver.Salary;
            EnterpriseId = driver.EnterpriseId;
            VehiclesIds = driver.DriversVehicles.Select(dv => dv.VehicleId).ToList();
            ActiveVehicleId = driver.ActiveVehicle?.Id;
        }
        public void AddSelectLists
        (
            ApplicationDbContext dbContext,
            ClaimsPrincipal claimsPrincipal,
            int userId
        )
        {
            var userEnterprises = Enterprise.GetUserEnterprises(dbContext, claimsPrincipal, userId);

            var vehiclesWithSameEnterprise = dbContext.Vehicles
                .Where(v => v.EnterpriseId == this.EnterpriseId);

            var vehiclesAttachedToDriver = dbContext.DriversVehicles
                .Where(dv => dv.DriverId == this.Id)
                .Include(dv => dv.Vehicle)
                .Select(dv => dv.Vehicle);

            this.EnterprisesSelectList = new SelectList(userEnterprises, "Id", "Name");
            this.VehiclesSelectList = new MultiSelectList(vehiclesWithSameEnterprise, "Id", "RegistrationNumber");
            this.ActiveVehicleSelectList = new SelectList(vehiclesAttachedToDriver, "Id", "RegistrationNumber");
        }
    }
}
