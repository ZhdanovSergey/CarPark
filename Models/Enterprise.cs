using System.Text.Json.Serialization;

namespace CarPark.Models
{
    public class Enterprise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        [JsonIgnore]
        public List<Vehicle> Vehicles { get; set; } = new();
        [JsonIgnore]
        public List<Driver> Drivers { get; set; } = new();
    }
}
