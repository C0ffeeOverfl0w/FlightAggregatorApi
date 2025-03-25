namespace FlightAggregator.Tests.Features.Flights.Queries;

public class SearchFlightsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsExpectedFlightsList()
    {
        // Arrange
        var departureDate = new DateTime(2023, 12, 25);
        var query = new SearchFlightsQuery(departureDate, "Test Airline", 150, 1);
        var handler = new SearchFlightsQueryHandler();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var flights = result.ToList();
        Assert.Equal(2, flights.Count);
        foreach (var flight in flights)
        {
            Assert.Equal(departureDate, flight.DepartureDate);
        }
    }
}