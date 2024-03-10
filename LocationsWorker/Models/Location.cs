using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkLocationsWorkerService.Models;

sealed class LocationDTO
{
    public required int VehicleId { get; init; }
    public required DateTime DateTime { get; init; }
    public required Point Point { get; init; }
}

sealed class Point
{
    public required double X { get; init; }
    public required double Y { get; init; }
}