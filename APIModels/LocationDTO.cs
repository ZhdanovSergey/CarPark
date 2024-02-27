namespace CarPark.APIModels;

public sealed class LocationDTO
{
    public int Id { get; init; }
    public required int VehicleId { get; init; }
    public required DateTimeOffset DateTime { get; init; }
    public required LocationDTOPoint Point { get; init; }
}

public sealed class LocationDTOPoint
{
    public required double X { get; init; }
    public required double Y { get; init; }
}