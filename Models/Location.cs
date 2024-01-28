using NetTopologySuite.Geometries;

namespace CarPark.Models;

public class Location
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public DateTime DateTime { get; set; }
    public Point Point { get; set; }
}
