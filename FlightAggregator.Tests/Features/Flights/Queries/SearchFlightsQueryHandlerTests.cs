namespace FlightAggregator.Tests.Features.Flights.Queries;

public class SearchFlightsQueryHandlerTests
{
    private readonly Mock<IFlightProvider> _flightProviderMock;
    private readonly Mock<IValidator<SearchFlightsQuery>> _validatorMock;
    private readonly SearchFlightsQueryHandler _handler;

    public SearchFlightsQueryHandlerTests()
    {
        _flightProviderMock = new Mock<IFlightProvider>();
        _validatorMock = new Mock<IValidator<SearchFlightsQuery>>();
        _handler = new SearchFlightsQueryHandler(_flightProviderMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsSortedFlightsByPriceAscending()
    {
        // Arrange
        var query = new SearchFlightsQuery
        (
            FlightNumber: null,
            Origin: "Origin",
            Destination: "Destination",
            DepartureTime: null,
            ArrivalTime: null,
            Airline: null,
            MaxPrice: null,
            MaxStops: null,
            Passengers: 1,
            SortBy: "price",
            SortOrder: "asc",
            PageNumber: 1,
            PageSize: 10
        );
        var flights = new List<Flight>
        {
            new Flight("FN123", DateTime.Now, DateTime.Now.AddHours(2), 120, new Airline(Guid.NewGuid(), "Airline1"), new Money(100, "USD"), 0, new List<Flight.StopDetailData>(), "Origin", "Destination", "Source"),
            new Flight("FN124", DateTime.Now, DateTime.Now.AddHours(3), 180, new Airline(Guid.NewGuid(), "Airline2"), new Money(200, "USD"), 0, new List<Flight.StopDetailData>(), "Origin", "Destination", "Source")
        };
        _flightProviderMock.Setup(x => x.GetFlightsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<decimal?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(flights);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(100, result.First().Price);
        Assert.Equal(200, result.Last().Price);
    }

    [Fact]
    public async Task Handle_ReturnsSortedFlightsByDateDescending()
    {
        // Arrange
        var query = new SearchFlightsQuery
        (
            FlightNumber: null,
            Origin: "Origin",
            Destination: "Destination",
            DepartureTime: null,
            ArrivalTime: null,
            Airline: null,
            MaxPrice: null,
            MaxStops: null,
            Passengers: 1,
            SortBy: "date",
            SortOrder: "desc",
            PageNumber: 1,
            PageSize: 10
        );
        var flights = new List<Flight>
        {
            new Flight("FN123", DateTime.Now, DateTime.Now.AddHours(2), 120, new Airline(Guid.NewGuid(), "Airline1"), new Money (100, "USD"), 0, new List<Flight.StopDetailData>(), "Origin", "Destination", "Source"),
            new Flight("FN124", DateTime.Now.AddHours(1), DateTime.Now.AddHours(1).AddHours(3), 180, new Airline(Guid.NewGuid(), "Airline2"), new Money (200, "USD"), 0, new List<Flight.StopDetailData>(), "Origin", "Destination", "Source")
        };
        _flightProviderMock.Setup(x => x.GetFlightsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<decimal?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(flights);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("FN124", result.First().FlightNumber);
        Assert.Equal("FN123", result.Last().FlightNumber);
    }
}