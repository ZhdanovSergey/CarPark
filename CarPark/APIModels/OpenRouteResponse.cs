namespace CarPark.APIModels;

sealed class OpenRouteResponse
{
    public required List<Feature> Features { get; init; }
}

sealed class Feature
{
    public required Properties Properties { get; init; }
}

sealed class Properties
{
    public required string Label { get; init; }
}