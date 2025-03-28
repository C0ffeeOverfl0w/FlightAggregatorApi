namespace FlightAggregator.Tests.Infrastructure.Providers;

public class FakeFlightProviderATests
{
    [Fact]
    public async Task GetFlightsAsync_RetriesOnTransientFailures_ReturnsFlight()
    {
        // Arrange
        var testHandler = new TestHttpMessageHandler(() =>
        {
            return "[ { " +
                   "\"Id\": \"1\", " +
                   "\"Airline\": \"Test Airline\", " +
                   "\"FlightNumber\": \"AB123\", " +
                   "\"Origin\": \"Москва\", " +
                   "\"Destination\": \"Санкт-Петербург\", " +
                   "\"DepartureTime\": \"2025-06-01T10:00:00Z\", " +
                   "\"ArrivalTime\": \"2025-06-01T12:00:00Z\", " +
                   "\"DurationMinutes\": 120, " +
                   "\"Price\": 100, " +
                   "\"Stops\": 0, " +
                   "\"StopDetails\": [], " +
                   "\"Source\": \"source1\"" +
                   " } ]";
        });

        // Создаем политику retry: 3 попытки с короткой задержкой (10 мс)
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(10));

        // Оборачиваем тестовый handler в PolicyHttpMessageHandler
        var policyHandler = new PolicyHttpMessageHandler(retryPolicy)
        {
            InnerHandler = testHandler
        };

        var httpClient = new HttpClient(policyHandler)
        {
            BaseAddress = new Uri("https://fake-flight-provider.wiremockapi.cloud")
        };

        var provider = new FakeFlightProviderA(httpClient);

        // Act
        var flights = await provider.GetFlightsAsync(
            flightNumber: null,
            origin: null,
            destination: null,
            departureTime: null,
            arrivalTime: null,
            airline: null,
            maxPrice: null,
            maxStops: null,
            passengers: 1,
            pageNumber: 1,
            pageSize: 10,
            CancellationToken.None);

        // Assert
        var flightList = flights.ToList();
        Assert.Single(flightList);
        Assert.Equal("AB123", flightList[0].FlightNumber);
        // Ожидаем, что тестовый handler был вызван 3 раза (2 сбоя + 1 успешный вызов)
        Assert.Equal(3, testHandler.CallCount);
    }
}