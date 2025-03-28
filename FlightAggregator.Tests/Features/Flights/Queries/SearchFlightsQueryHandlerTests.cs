namespace FlightAggregator.Tests.Features.Flights.Queries;

public class SearchFlightsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsExpectedFlightsList_WithCompositeProvider()
    {
        // Arrange
        var departureDate = new DateTime(2023, 12, 25);
        var query = new SearchFlightsQuery(
            FlightNumber: "2525",
            DepartureDate: departureDate,
            Airline: "Test Airline",
            MaxPrice: 150,
            MaxStops: 1,
            SortBy: null,
            SortOrder: null,
            PageNumber: 1,
            PageSize: 10
        );

        // Мок провайдера A, который вернёт 2 рейса
        var providerAMock = new Mock<IFlightProvider>();
        providerAMock
            .Setup(p => p.GetFlightsAsync(
                query.FlightNumber,
                query.DepartureDate,
                query.Airline,
                query.MaxPrice,
                query.MaxStops,
                query.PageNumber,
                query.PageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Flight>
            {
            new Flight("2525", departureDate, new Airline(Guid.NewGuid(), "Test Airline"), new Money(100, "USD"), 0),
            new Flight("2526", departureDate, new Airline(Guid.NewGuid(), "Test Airline"), new Money(120, "USD"), 1)
            });

        // Мок провайдера B, возвращающий пустой список
        var providerBMock = new Mock<IFlightProvider>();
        providerBMock
            .Setup(p => p.GetFlightsAsync(
                query.FlightNumber,
                query.DepartureDate,
                query.Airline,
                query.MaxPrice,
                query.MaxStops,
                query.PageNumber,
                query.PageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Flight>());

        // Композитный провайдер объединяет данные от обоих провайдеров
        var compositeProvider = new CompositeFlightProvider([providerAMock.Object, providerBMock.Object]);

        // Создаем мок валидатора
        var validatorMock = new Mock<IValidator<SearchFlightsQuery>>();

        // Инжектируем композитный провайдер и валидатор в обработчик
        var handler = new SearchFlightsQueryHandler(compositeProvider, validatorMock.Object);

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