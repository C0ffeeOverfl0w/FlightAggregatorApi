namespace FlightAggregator.Infrastructure.Providers;

public class FakeFlightProviderB(HttpClient httpClient) : BaseFakeFlightProvider(httpClient)
{
}