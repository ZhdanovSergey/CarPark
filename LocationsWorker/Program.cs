using CommandLine;

namespace CarParkLocationsWorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(Run)
            .WithNotParsed(HandleParseError);
    }
    static void Run(Options options)
    {
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(options);
                services.AddHostedService<Worker>();
            })
            .Build();

        host.Run();
    }
    static void HandleParseError(IEnumerable<Error> errs)
    {
        Console.WriteLine("Parser Fail");
    }
}