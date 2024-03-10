using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CarParkLocationsWorkerService.Models;

class ApiResponse
{
    [JsonPropertyName("routes")]
    public List<Route> Routes { get; init; }
}

class Route
{
    [JsonPropertyName("geometry")]
    public string Geometry { get; init; }

    [JsonPropertyName("segments")]
    public List<Segment> Segments { get; init; }
}

class Segment
{
    [JsonPropertyName("steps")]
    public List<Step> Steps { get; init; }
}

class Step
{
    [JsonPropertyName("distance")]
    public double Distance { get; init; }

    [JsonPropertyName("duration")]
    public double Duration { get; init; }

    [JsonPropertyName("way_points")]
    public List<int> WayPoints { get; init; }
}