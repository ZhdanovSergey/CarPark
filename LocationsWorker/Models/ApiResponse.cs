using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CarParkLocationsWorkerService.Models;

sealed class ApiResponse
{
    [JsonPropertyName("routes")]
    public required List<Route> Routes { get; init; }
}

sealed class Route
{
    [JsonPropertyName("geometry")]
    public required string Geometry { get; init; }

    [JsonPropertyName("segments")]
    public required List<Segment> Segments { get; init; }
}

sealed class Segment
{
    [JsonPropertyName("steps")]
    public required List<Step> Steps { get; init; }
}

sealed class Step
{
    [JsonPropertyName("distance")]
    public required double Distance { get; init; }

    [JsonPropertyName("duration")]
    public required double Duration { get; init; }

    [JsonPropertyName("way_points")]
    public required List<int> WayPoints { get; init; }
}