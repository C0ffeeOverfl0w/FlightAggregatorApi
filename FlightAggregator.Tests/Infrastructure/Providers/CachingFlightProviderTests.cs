namespace FlightAggregator.Tests.Infrastructure.Providers;

public class CachingFlightProviderTests
{
    [Fact]
    public async Task GetFlightsAsync_CachesResult()
    {
        // Arrange
        var testFlight = new Flight(
            id: "1",
            flightNumber: "FN123",
            departureTime: new DateTime(2025, 6, 15, 8, 30, 0),
            arrivalTime: new DateTime(2025, 6, 15, 10, 0, 0),
            durationMinutes: 90,
            airline: new Airline(Guid.NewGuid(), "Аэрофлот"),
            price: new Money(5600, "USD"),
            stops: 0,
            stopDetails: new List<Flight.StopDetailData>(),
            origin: "Москва (SVO)",
            destination: "Санкт-Петербург (LED)",
            source: "source1"
        );
        var flightList = new List<Flight> { testFlight };

        var mockInner = new Mock<IFlightProvider>();
        mockInner.Setup(x => x.GetFlightsAsync(
            It.IsAny<string?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<string?>(),
            It.IsAny<decimal?>(),
            It.IsAny<int?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(flightList);

        // Создаем IMemoryCache
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var cachingProvider = new CachingFlightProvider(mockInner.Object, memoryCache);

        string? flightNumber = null;
        string origin = "Москва (SVO)";
        string destination = "Санкт-Петербург (LED)";
        DateTime? departureTime = new DateTime(2025, 6, 15, 8, 30, 0);
        DateTime? arrivalTime = new DateTime(2025, 6, 15, 10, 0, 0);
        string? airline = null;
        decimal? maxPrice = null;
        int? maxStops = null;
        int passengers = 1;
        int pageNumber = 1;
        int pageSize = 10;

        // Act: Первый вызов – кэш не заполнен, внутренний провайдер должен быть вызван
        var result1 = await cachingProvider.GetFlightsAsync(
            flightNumber, origin, destination, departureTime, arrivalTime,
            airline, maxPrice, maxStops, passengers, pageNumber, pageSize, CancellationToken.None);

        // Act: Второй вызов – результат должен быть взят из кэша
        var result2 = await cachingProvider.GetFlightsAsync(
            flightNumber, origin, destination, departureTime, arrivalTime,
            airline, maxPrice, maxStops, passengers, pageNumber, pageSize, CancellationToken.None);

        // Assert: Результаты должны совпадать
        Assert.Equal(result1, result2);

        // Assert
        mockInner.Verify(x => x.GetFlightsAsync(
            flightNumber, origin, destination, departureTime, arrivalTime,
            airline, maxPrice, maxStops, passengers, pageNumber, pageSize,
            It.IsAny<CancellationToken>()),
            Times.Once());
    }
}