using CarPark.Models;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ViewModels
{
    public class DriverApiViewModel
    {
        public int Id { get; set; }
        public int EnterpriseId { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        public int? ActiveVehicleId { get; set; }
        public List<int> VehiclesIds { get; set; }
        public DriverApiViewModel(Driver driver)
        {
            Id = driver.Id;
            EnterpriseId = driver.EnterpriseId;
            Name = driver.Name;
            Salary = driver.Salary;
            ActiveVehicleId = driver.ActiveVehicle?.Id;
            VehiclesIds = driver.DriversVehicles.Select(dv => dv.VehicleId).ToList();
        }
    }
}
