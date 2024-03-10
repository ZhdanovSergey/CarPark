using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkLocationsWorkerService;

public class Options
{
    static readonly Random _random = new();
    static double GetRandomDouble(double min, double max) =>
        _random.NextDouble() * (max - min) + min;

    [Option("id", Required = true)]
    public int VehicleId { get; init; }

    [Option('d', "distance")]
    public int Distance { get; init; } = 10_000;

    [Option('x', "longitude")]
    public double Longitude { get; init; } = GetRandomDouble(82.7, 83.0);

    [Option('y', "latitude")]
    public double Latitude { get; init; } = GetRandomDouble(54.9, 55.1);
}