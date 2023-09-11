using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CarPark.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public int EnterpriseId { get; set; }
        [NotMapped]
        public int? ActiveVehicleId { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        [JsonIgnore]
        public Enterprise? Enterprise { get; set; }
        [JsonIgnore]
        public Vehicle? ActiveVehicle { get; set; }
        [JsonIgnore]
        public List<DriverVehicle> DriversVehicles { get; set; } = new();
        [JsonIgnore]
        [NotMapped]
        public List<int> SelectedVehiclesIds { get; set; } = new();
    }
}
