using CarPark.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPark.APIModels
{
    public class EnterpriseAPIModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public List<int> DriversIds { get; set; } = new();
        public List<int> VehiclesIds { get; set; } = new();
        public EnterpriseAPIModel(Enterprise enterprise)
        {
            Id = enterprise.Id;
            Name = enterprise.Name;
            City = enterprise.City;
            DriversIds = enterprise.Drivers.Select(d => d.Id).ToList();
            VehiclesIds = enterprise.Vehicles.Select(v => v.Id).ToList();
        }
    }
}
