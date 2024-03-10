using NetTopologySuite.Geometries;

namespace CarPark.Models;

public sealed class Location
{
    public int Id { get; init; }
    public required int VehicleId { get; init; }
    public Vehicle? Vehicle { get; init; }
    public required DateTime DateTime { get; init; }
    public required Point Point { get; init; }
}