namespace FlightAggregator.Infrastructure.Providers;

/// <summary>
/// Фабрика для получения экземпляра IFlightProvider в зависимости от типа провайдера.
/// </summary>
public class FlightProviderFactory : IFlightProviderFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public FlightProviderFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IFlightProvider GetFlightProvider(FlightProviderType type) => type switch
    {
        FlightProviderType.ProviderA => new FakeFlightProviderA(_httpClientFactory.CreateClient(nameof(FakeFlightProviderA))),
        FlightProviderType.ProviderB => new FakeFlightProviderB(_httpClientFactory.CreateClient(nameof(FakeFlightProviderB))),
        _ => throw new ArgumentException("Неизвестный тип провайдера", nameof(type))
    };
}