namespace CarPark.Models;

public sealed class Ride
{
    public int Id { get; init; }
    public required int VehicleId { get; init; }
    public Vehicle? Vehicle { get; init; }
    public required DateTime Start { get; init; }
    public required DateTime End { get; init; }
}
