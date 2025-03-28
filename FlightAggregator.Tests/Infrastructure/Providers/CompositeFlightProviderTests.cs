using static FlightAggregator.Application.Interfaces.IFlightProvider;

namespace FlightAggregator.Tests.Infrastructure.Providers;

public class CompositeFlightProviderTests
{
    private readonly Mock<IFlightProvider> _providerMock1;
    private readonly Mock<IFlightProvider> _providerMock2;
    private readonly CompositeFlightProvider _compositeFlightProvider;

    public CompositeFlightProviderTests()
    {
        _providerMock1 = new Mock<IFlightProvider>();
        _providerMock2 = new Mock<IFlightProvider>();
        var providers = new List<IFlightProvider> { _providerMock1.Object, _providerMock2.Object };
        _compositeFlightProvider = new CompositeFlightProvider(providers);
    }

    [Fact]
    public async Task GetFlightsAsync_ReturnsCombinedResults()
    {
        // Arrange
        var flight1 = new Flight("1", "FL123", DateTime.Now, DateTime.Now.AddHours(2), 120, new Airline(Guid.NewGuid(), "Airline1"), new Money(100, "USD"), 0, new List<Flight.StopDetailData>(), "Origin1", "Destination1", "Source1");
        var flight2 = new Flight("2", "FL456", DateTime.Now, DateTime.Now.AddHours(3), 180, new Airline(Guid.NewGuid(), "Airline2"), new Money(200, "USD"), 1, new List<Flight.StopDetailData>(), "Origin2", "Destination2", "Source2");

        _providerMock1.Setup(p => p.GetFlightsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<decimal?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new List<Flight> { flight1 });

        _providerMock2.Setup(p => p.GetFlightsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<decimal?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new List<Flight> { flight2 });

        // Act
        var result = await _compositeFlightProvider.GetFlightsAsync(null, "Origin", "Destination", null, null, null, null, null, 1, 1, 1, CancellationToken.None);

        // Assert
        Assert.Contains(flight1, result);
        Assert.Contains(flight2, result);
    }

    [Fact]
    public async Task BookFlightAsync_ReturnsBookingResponse()
    {
        // Arrange
        var bookingRequest = new BookingRequest("FL123", "John Doe");
        var bookingResponse = new BookingResponse("1", "FL123", "John Doe", DateTime.Now, true, "Success");

        _providerMock1.Setup(p => p.GetFlightsAsync(It.IsAny<string>(), null, null, null, null, null, null, null, 1, 1, 1, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new List<Flight> { new Flight("1", "FL123", DateTime.Now, DateTime.Now.AddHours(2), 120, new Airline(Guid.NewGuid(), "Airline1"), new Money(100, "USD"), 0, new List<Flight.StopDetailData>(), "Origin1", "Destination1", "Source1") });

        _providerMock1.Setup(p => p.BookFlightAsync(bookingRequest, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(bookingResponse);

        // Act
        var result = await _compositeFlightProvider.BookFlightAsync(bookingRequest, CancellationToken.None);

        // Assert
        Assert.Equal(bookingResponse, result);
    }

    [Fact]
    public async Task CancelBookingAsync_ThrowsNotImplementedException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _compositeFlightProvider.CancelBookingAsync("1", CancellationToken.None));
    }
}