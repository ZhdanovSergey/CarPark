using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommandLine;

namespace CarParkGenerator;

class Options
{
    [Option('e', "enterprise-ids", Required = true)]
    public IEnumerable<int> EnterpriseIds { get; set; }
    [Option('v', "vehicles")]
    public int Vehicles { get; set; }
    [Option('d', "drivers")]
    public int Drivers { get; set; }
    [Option('o', "output")]
    public string OutputFileName { get; set; }
}