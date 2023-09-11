using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CarPark.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        [Column("DriverId")]
        public int? ActiveDriverId { get; set; }
        public int EnterpriseId { get; set; }
        public string RegistrationNumber { get; set; }
        public int Mileage { get; set; }
        public int Price { get; set; }
        public int Year { get; set; }
        [JsonIgnore]
        public Brand? Brand { get; set; }
        [JsonIgnore]
        [ForeignKey("ActiveDriverId")]
        public Driver? ActiveDriver { get; set; }
        [JsonIgnore]
        public Enterprise? Enterprise { get; set; }
        [JsonIgnore]
        public List<DriverVehicle> DriversVehicles { get; set; } = new();
        [JsonIgnore]
        [NotMapped]
        public List<int> SelectedDriversIds { get; set; } = new();
    }
}
