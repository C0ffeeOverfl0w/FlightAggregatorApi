namespace FlightAggregator.Tests.Infrastructure.Providers;

public class FakeFlightProviderATests
{
    [Fact]
    public async Task GetFlightsAsync_RetriesOnTransientFailures_ReturnsFlight()
    {
        // Arrange
        var testHandler = new TestHttpMessageHandler(() =>
        {
            // Возвращаем желаемый JSON-ответ для теста
            return "[ { \"FlightNumber\": \"AB123\" } ]";
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