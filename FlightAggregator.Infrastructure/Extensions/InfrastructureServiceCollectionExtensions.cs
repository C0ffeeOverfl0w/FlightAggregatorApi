namespace FlightAggregator.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrlA = configuration["FlightProviders:ProviderA:BaseUrl"];
        var baseUrlB = configuration["FlightProviders:ProviderB:BaseUrl"];

        if (string.IsNullOrEmpty(baseUrlA))
        {
            throw new ArgumentNullException(nameof(baseUrlA), "Base URL for Provider A is not configured.");
        }

        if (string.IsNullOrEmpty(baseUrlB))
        {
            throw new ArgumentNullException(nameof(baseUrlB), "Base URL for Provider B is not configured.");
        }

        services.AddHttpClient<FakeFlightProviderA>(client =>
        {
            client.BaseAddress = new Uri(baseUrlA);
        });
        services.AddScoped<FakeFlightProviderA>();

        services.AddHttpClient<FakeFlightProviderB>(client =>
        {
            client.BaseAddress = new Uri(baseUrlB);
        });
        services.AddScoped<FakeFlightProviderB>();

        services.AddScoped<IFlightProviderFactory, FlightProviderFactory>();

        return services;
    }
}