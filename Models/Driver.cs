using CarPark.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CarPark.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public int EnterpriseId { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        [NotMapped]
        public int? ActiveVehicleId
        {
            get
            {
                return this.ActiveVehicle?.Id;
            }
        }
        [NotMapped]
        public List<int> VehiclesIds
        {
            get
            {
                return this.DriversVehicles.Select(dv => dv.VehicleId).ToList();
            }
        }
        [JsonIgnore]
        public Vehicle? ActiveVehicle { get; set; }
        [JsonIgnore]
        public Enterprise? Enterprise { get; set; }
        [JsonIgnore]
        public List<DriverVehicle> DriversVehicles { get; set; } = new();
        public Driver() { }
        public Driver(DriverEditViewModel driverEdit)
        {
            Id = driverEdit.Id;
            Name = driverEdit.Name;
            Salary = driverEdit.Salary;
            EnterpriseId = driverEdit.EnterpriseId;
        }
    }
}
