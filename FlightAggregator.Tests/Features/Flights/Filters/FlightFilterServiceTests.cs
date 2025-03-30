namespace FlightAggregator.Tests.Features.Flights.Filters;

public class FlightFilterServiceTests
{
    private readonly IFlightFilterService _filterService;
    private readonly List<IFlightFilter> _filters;

    public FlightFilterServiceTests()
    {
        _filters = new List<IFlightFilter>
    {
        new OriginFilter(),
        new DestinationFilter(),
        new DateFilter()
    };

        _filterService = new FlightFilterService(_filters);
    }

    [Theory]
    [InlineData("Moscow", "London", 1)]
    [InlineData("Moscow", null, 2)]
    [InlineData("Berlin", null, 0)]
    public void ApplyFilters_ShouldCorrectlyFilterByOriginAndDestination(
        string origin,
        string? destination,
        int expectedCount)
    {
        // Arrange
        var flights = new List<Flight>
    {
        new Flight("FL001", DateTime.Now, DateTime.Now.AddHours(2), 120,
            new Airline(null, "Aeroflot"), new Money(5000, "RUB"), 0,
            new List<Flight.StopDetailData>(), "Moscow", "London", "ProviderA"),

        new Flight("FL002", DateTime.Now, DateTime.Now.AddHours(2), 120,
            new Airline(null, "S7"), new Money(4000, "RUB"), 0,
            new List<Flight.StopDetailData>(), "Moscow", "Paris", "ProviderB")
    };

        var query = new SearchFlightsQuery(
            null,
            Origin: origin,
            Destination: destination,
            DepartureTime: null,
            SortBy: null,
            SortOrder: null);

        // Act
        var result = _filterService.ApplyFilters(flights, query);

        // Assert
        Assert.Equal(expectedCount, result.Count());
    }
}