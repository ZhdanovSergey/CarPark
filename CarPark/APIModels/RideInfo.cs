using CarPark.Models;

namespace CarPark.APIModels;

public sealed class RideInfo
{
    public int Id { get; init; }
    public required int VehicleId { get; init; }
    public required RideInfoEdge? Start { get; init; }
    public required RideInfoEdge? End { get; init; }
}

public sealed class RideInfoEdge
{
    public required DateTimeOffset DateTime { get; init; }
    public required PointDTO Point { get; init; }
    public required string PhysicalAdress { get; init; }
}