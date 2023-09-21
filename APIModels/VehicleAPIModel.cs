using CarPark.Models;
using CarPark.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPark.APIModels
{
    public class VehicleAPIModel
    {
        public int Id { get; set; }
        public int? ActiveDriverId { get; set; }
        public int BrandId { get; set; }
        public int EnterpriseId { get; set; }
        public string RegistrationNumber { get; set; }
        public int Mileage { get; set; }
        public int Price { get; set; }
        public int Year { get; set; }
        public List<int> DriversIds { get; set; } = new();
        public VehicleAPIModel(Vehicle vehicle)
        {
            Id = vehicle.Id;
            ActiveDriverId = vehicle.ActiveDriverId;
            BrandId = vehicle.BrandId;
            EnterpriseId = vehicle.EnterpriseId;
            RegistrationNumber = vehicle.RegistrationNumber;
            Mileage = vehicle.Mileage;
            Price = vehicle.Price;
            Year = vehicle.Year;
            DriversIds = vehicle.DriversVehicles.Select(dv => dv.DriverId).ToList();
        }
    }
}
