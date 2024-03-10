using CarParkGenerator.Models;
using CommandLine;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace CarParkGenerator
{
    internal class Program
    {
        static Random random = new();
        static readonly string[] names = new string[] { "Emily", "David", "Samantha", "William", "Olivia", "Michael", "Sophia", "Benjamin", "Ava", "Ethan" };
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(HandleParseError);
        }

        static void Run(Options options)
        {
            var drivers = new List<DriverModel>();
            var vehicles = new List<VehicleModel>();

            foreach (var enterpriseId in options.EnterpriseIds)
            {
                for (int i = 0; i < options.Drivers; i++)
                {
                    drivers.Add(new DriverModel()
                    {
                        EnterpriseId = enterpriseId,
                        Name = names[random.Next(0, names.Length)],
                        Salary = random.Next(100, 1000) * 1000
                    });
                }

                for (int i = 0; i < options.Vehicles; i++)
                {
                    var hasActiveDriver = random.Next(0, 10) == 0;

                    vehicles.Add(new VehicleModel()
                    {
                        EnterpriseId = enterpriseId,
                        BrandId = random.Next(1, 3),
                        ActiveDriverId = hasActiveDriver ? 3000 * (enterpriseId - 1) + i + 3008 : null,
                        RegistrationNumber = String.Format("{0}{1}{2}{3}{4}{5}",
                            (char)random.Next('A', 'Z' + 1),
                            random.Next(0, 10),
                            random.Next(0, 10),
                            random.Next(0, 10),
                            (char)random.Next('A', 'Z' + 1),
                            (char)random.Next('A', 'Z' + 1)
                        ),
                        Mileage = random.Next(0, 1000) * 1000,
                        Price = random.Next(100, 10000) * 1000,
                        Year = random.Next(1973, 2023)
                    });
                }
            }

            var jsonResult = JsonSerializer.Serialize(new { Drivers = drivers, Vehicles = vehicles });

            if (options.OutputFileName is not null)
                File.WriteAllText(options.OutputFileName, jsonResult);
            else
                Console.WriteLine(jsonResult);
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Parser Fail");
        }
    }
}