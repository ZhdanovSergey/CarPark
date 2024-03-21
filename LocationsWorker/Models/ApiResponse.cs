using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkLocationsWorkerService.Models;

sealed class ApiResponse
{
    public required List<Route> Routes { get; init; }
}

sealed class Route
{
    public required string Geometry { get; init; }
    public required List<Segment> Segments { get; init; }
}

sealed class Segment
{
    public required List<Step> Steps { get; init; }
}

sealed class Step
{
    public required double Distance { get; init; }
    public required double Duration { get; init; }
    public required List<int> WayPoints { get; init; }
}