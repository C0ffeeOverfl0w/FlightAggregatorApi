namespace FlightAggregator.Tests.Infrastructure.Providers;

public class CompositeFlightProviderTests
{
    [Fact]
    public async Task GetFlightsAsync_AggregatesResultsFromAllProviders_ReturnsUnifiedData()
    {
        // Arrange
        var testHandlerA = new TestHttpMessageHandler(() =>
        {
            return "[ { \"FlightNumber\": \"AB123\", \"DepartureDate\": \"2025-05-01T10:00:00Z\", \"AirlineName\": \"Fake Airline A\", \"Price\": 150, \"Stops\": 0 } ]";
        });

        var testHandlerB = new TestHttpMessageHandler(() =>
        {
            return "[ { \"FlightId\": \"CD456\", \"DepDate\": \"2025-05-01T12:00:00Z\", \"Airline\": \"Fake Airline B\", \"Cost\": 200, \"Connections\": 1 } ]";
        });

        var httpClientA = new HttpClient(testHandlerA)
        {
            BaseAddress = new Uri("https://fake-flight-provider-a.wiremockapi.cloud")
        };
        var httpClientB = new HttpClient(testHandlerB)
        {
            BaseAddress = new Uri("https://fake-flight-provider-b.wiremockapi.cloud")
        };

        var providerA = new FakeFlightProviderA(httpClientA);
        var providerB = new FakeFlightProviderB(httpClientB);

        var compositeProvider = new CompositeFlightProvider([providerA, providerB]);

        // Act
        var flights = await compositeProvider.GetFlightsAsync(
            flightNumber: null,
            departureDate: null,
            airline: null,
            maxPrice: null,
            maxStops: null,
            pageNumber: 1,
            pageSize: 10,
            CancellationToken.None);
        var flightList = flights.ToList();

        // Assert
        Assert.Equal(2, flightList.Count);

        var flightA = flightList.FirstOrDefault(f => f.FlightNumber == "AB123");
        Assert.NotNull(flightA);
        Assert.Equal("Fake Airline A", flightA.Airline.Name);
        Assert.Equal(150, flightA.Price.Amount);

        var flightB = flightList.FirstOrDefault(f => f.FlightNumber == "CD456");
        Assert.NotNull(flightB);
        Assert.Equal("Fake Airline B", flightB.Airline.Name);
        Assert.Equal(200, flightB.Price.Amount);
    }
}