namespace FlightAggregator.Infrastructure.Logging;

public static class SerilogExtensions
{
    public static void AddSerilogLogging(this IHostBuilder hostBuilder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} (Thread: {ThreadId}){NewLine}{Exception}")
            .CreateLogger();

        hostBuilder.UseSerilog();
    }
}