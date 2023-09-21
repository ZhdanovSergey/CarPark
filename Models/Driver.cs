using CarPark.ViewModels;

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
    }
}
