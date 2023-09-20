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
        public static async Task<DriverEditViewModel> CreateNewAsync (Driver driver, AppDbContext dbContext)
        {
            var vehiclesIds = await dbContext.DriversVehicles
                .Where(dv => dv.DriverId == driver.Id)
                .Select(dv => dv.VehicleId)
                .ToListAsync();

            var activeVehicleId = (await dbContext.Drivers
                .Include(d => d.ActiveVehicle)
                .FirstOrDefaultAsync(d => d.Id == driver.Id))?.ActiveVehicle?.Id;

            var selectLists = await CreateSelectLists(dbContext, driver.Id, driver.EnterpriseId);

            return new DriverEditViewModel()
            {
                Id = driver.Id,
                Name = driver.Name,
                Salary = driver.Salary,
                EnterpriseId = driver.EnterpriseId,
                VehiclesIds = vehiclesIds,
                ActiveVehicleId = activeVehicleId,
                EnterprisesSelectList = selectLists.EnterprisesSelectList,
                VehiclesSelectList = selectLists.VehiclesSelectList,
                ActiveVehicleSelectList = selectLists.ActiveVehicleSelectList,
            };
        }
        public static async Task<DriverEditViewModel> CreateNewAsync(DriverEditViewModel driverEdit, AppDbContext dbContext)
        {
            var selectLists = await CreateSelectLists(dbContext, driverEdit.Id, driverEdit.EnterpriseId);

            return new DriverEditViewModel()
            {
                Id = driverEdit.Id,
                Name = driverEdit.Name,
                Salary = driverEdit.Salary,
                EnterpriseId = driverEdit.EnterpriseId,
                VehiclesIds = driverEdit.VehiclesIds,
                ActiveVehicleId = driverEdit.ActiveVehicleId,
                EnterprisesSelectList = selectLists.EnterprisesSelectList,
                VehiclesSelectList = selectLists.VehiclesSelectList,
                ActiveVehicleSelectList = selectLists.ActiveVehicleSelectList,

            };
        }
        static async Task<SelectLists> CreateSelectLists(AppDbContext dbContext, int driverId, int enterpriseId)
        {
            var vehiclesWithSameEnterprise = await dbContext.Vehicles
                .Where(v => v.EnterpriseId == enterpriseId)
                .ToListAsync();

            var vehiclesAttachedToDriver = await dbContext.DriversVehicles
                .Where(dv => dv.DriverId == driverId)
                .Include(dv => dv.Vehicle)
                .Select(dv => dv.Vehicle)
                .ToListAsync();

            return new SelectLists()
            {
                EnterprisesSelectList = new SelectList(dbContext.Enterprises, "Id", "Name"),
                VehiclesSelectList = new MultiSelectList(vehiclesWithSameEnterprise, "Id", "RegistrationNumber"),
                ActiveVehicleSelectList = new SelectList(vehiclesAttachedToDriver, "Id", "RegistrationNumber"),
            };
        }
    }
    class SelectLists
    {
        public SelectList EnterprisesSelectList;
        public MultiSelectList VehiclesSelectList;
        public SelectList ActiveVehicleSelectList;
    }
}
