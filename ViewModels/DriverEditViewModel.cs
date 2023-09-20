using CarPark.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DriverEditViewModel(Driver driver, AppDbContext dbContext)
        {
            var vehiclesIds = dbContext.DriversVehicles
                .Where(dv => dv.DriverId == driver.Id)
                .Select(dv => dv.VehicleId)
                .ToList();

            var activeVehicleId = dbContext.Drivers
                .Include(d => d.ActiveVehicle)
                .FirstOrDefault(d => d.Id == driver.Id)?.ActiveVehicle?.Id;

            CreateSelectLists(dbContext, driver.Id, driver.EnterpriseId,
                out SelectList EnterprisesSelectList,
                out MultiSelectList VehiclesSelectList,
                out SelectList ActiveVehicleSelectList);

            this.Id = driver.Id;
            this.Name = driver.Name;
            this.Salary = driver.Salary;
            this.EnterpriseId = driver.EnterpriseId;
            this.VehiclesIds = vehiclesIds;
            this.ActiveVehicleId = activeVehicleId;
            this.EnterprisesSelectList = EnterprisesSelectList;
            this.VehiclesSelectList = VehiclesSelectList;
            this.ActiveVehicleSelectList = ActiveVehicleSelectList;
        }
        public DriverEditViewModel(DriverEditViewModel driverEdit, AppDbContext dbContext)
        {
            CreateSelectLists(dbContext, driverEdit.Id, driverEdit.EnterpriseId,
                out SelectList EnterprisesSelectList,
                out MultiSelectList VehiclesSelectList,
                out SelectList ActiveVehicleSelectList);

            this.Id = driverEdit.Id;
            this.Name = driverEdit.Name;
            this.Salary = driverEdit.Salary;
            this.EnterpriseId = driverEdit.EnterpriseId;
            this.VehiclesIds = driverEdit.VehiclesIds;
            this.ActiveVehicleId = driverEdit.ActiveVehicleId;
            this.EnterprisesSelectList = EnterprisesSelectList;
            this.VehiclesSelectList = VehiclesSelectList;
            this.ActiveVehicleSelectList = ActiveVehicleSelectList;
        }
        static void CreateSelectLists(AppDbContext dbContext, int driverId, int enterpriseId,
            out SelectList EnterprisesSelectList,
            out MultiSelectList VehiclesSelectList,
            out SelectList ActiveVehicleSelectList)
        {
            var vehiclesWithSameEnterprise = dbContext.Vehicles
                .Where(v => v.EnterpriseId == enterpriseId);

            var vehiclesAttachedToDriver = dbContext.DriversVehicles
                .Where(dv => dv.DriverId == driverId)
                .Include(dv => dv.Vehicle)
                .Select(dv => dv.Vehicle);

            EnterprisesSelectList = new SelectList(dbContext.Enterprises, "Id", "Name");
            VehiclesSelectList = new MultiSelectList(vehiclesWithSameEnterprise, "Id", "RegistrationNumber");
            ActiveVehicleSelectList = new SelectList(vehiclesAttachedToDriver, "Id", "RegistrationNumber");
        }
    }
}
