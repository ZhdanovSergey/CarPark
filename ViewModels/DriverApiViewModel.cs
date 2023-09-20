using CarPark.Models;
using System.ComponentModel.DataAnnotations.Schema;

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
    }
}
