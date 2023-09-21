using CarPark.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;
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
    }
}
