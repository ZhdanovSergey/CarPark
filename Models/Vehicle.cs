using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CarPark.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        [JsonIgnore]
        public Brand? Brand { get; set; }
        public int Mileage { get; set; }
        public int Price { get; set; }
        public string RegistrationNumber { get; set; }
        public int Year { get; set; }
    }
}
