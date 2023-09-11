namespace CarPark.Models
{
    public class DriverVehicle
    {
        public int EnterpriseId { get; set; }
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public Enterprise? Enterprise { get; set; }
        public Driver? Driver { get; set; }
        public Vehicle? Vehicle { get; set; }
    }
}
