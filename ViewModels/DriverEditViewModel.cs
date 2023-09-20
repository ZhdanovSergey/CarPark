using CarPark.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ViewModels
{
    public class DriverEditViewModel
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
        public DriverEditViewModel() { }
        public DriverEditViewModel(Driver driver)
        {
            Id = driver.Id;
            Name = driver.Name;
            Salary = driver.Salary;
            EnterpriseId = driver.EnterpriseId;
            VehiclesIds = driver.DriversVehicles.Select(dv => dv.VehicleId).ToList();
            ActiveVehicleId = driver.ActiveVehicle?.Id;
        }
        public static explicit operator Driver (DriverEditViewModel driverEdit) => new()
        {
            Id = driverEdit.Id,
            Name = driverEdit.Name,
            Salary = driverEdit.Salary,
            EnterpriseId = driverEdit.EnterpriseId,
        };
        public async Task AddSelectLists(AppDbContext dbContext)
        {
            var vehiclesWithSameEnterprise = await dbContext.Vehicles
                .Where(v => v.EnterpriseId == this.EnterpriseId)
                .ToListAsync();

            var vehiclesAttachedToDriver = await dbContext.DriversVehicles
                .Where(dv => dv.DriverId == this.Id)
                .Include(dv => dv.Vehicle)
                .Select(dv => dv.Vehicle)
                .ToListAsync();

            this.EnterprisesSelectList = new SelectList(await dbContext.Enterprises.ToListAsync(), "Id", "Name");
            this.VehiclesSelectList = new MultiSelectList(vehiclesWithSameEnterprise, "Id", "RegistrationNumber");
            this.ActiveVehicleSelectList = new SelectList(vehiclesAttachedToDriver, "Id", "RegistrationNumber");
        }
    }
}
