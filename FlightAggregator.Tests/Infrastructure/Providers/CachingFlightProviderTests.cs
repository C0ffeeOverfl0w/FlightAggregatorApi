namespace FlightAggregator.Tests.Infrastructure.Providers;

public class CachingFlightProviderTests
{
    [Fact]
    public async Task GetFlightsAsync_CachesResult()
    {
        // Arrange
        var testFlight = new Flight(
            flightNumber: "FN123",
            departureTime: new DateTime(2025, 6, 15, 8, 30, 0),
            arrivalTime: new DateTime(2025, 6, 15, 10, 0, 0),
            durationMinutes: 90,
            airline: new Airline(null, "Аэрофлот"),
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
            It.IsAny<SearchFlightsQuery>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(flightList);

        // Создаем IMemoryCache
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var cachingProvider = new CachingFlightProvider(mockInner.Object, memoryCache);

        var query = new SearchFlightsQuery(
            FlightNumber: null,
            Origin: "Москва (SVO)",
            Destination: "Санкт-Петербург (LED)",
            DepartureTime: new DateTime(2025, 6, 15, 8, 30, 0),
            ArrivalTime: new DateTime(2025, 6, 15, 10, 0, 0),
            Airline: null,
            MaxPrice: null,
            MaxStops: null,
            Passengers: 1,
            SortBy: null,
            SortOrder: null,
            PageNumber: 1,
            PageSize: 10
        );

        // Act: Первый вызов – кэш не заполнен, внутренний провайдер должен быть вызван
        var result1 = await cachingProvider.GetFlightsAsync(
            query, CancellationToken.None);

        // Act: Второй вызов – результат должен быть взят из кэша
        var result2 = await cachingProvider.GetFlightsAsync(
            query, CancellationToken.None);

        // Assert: Результаты должны совпадать
        Assert.Equal(result1, result2);

        // Assert
        mockInner.Verify(x => x.GetFlightsAsync(
            query, It.IsAny<CancellationToken>()),
            Times.Once());
    }
}