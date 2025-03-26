namespace FlightAggregator.Infrastructure.Providers;

/// <summary>
/// Фейковый провайдер А для получения информации о рейсах.
/// </summary>
public class FakeFlightProviderA(HttpClient httpClient) : BaseFakeFlightProvider(httpClient)
{
}