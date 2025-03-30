namespace FlightAggregator.Tests.Infrastructure.Providers;

public class FakeFlightProviderATests
{
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    private readonly Mock<ILogger<FakeFlightProviderA>> _loggerMock = new Mock<ILogger<FakeFlightProviderA>>();

    public FakeFlightProviderATests()
    {
        _mapperMock = (Mock<IMapper>)_mapperMock.Object;
    }

    [Fact]
    public async Task GetFlightsAsync_RetriesOnServerError_ReturnsFlight()
    {
        // Arrange: возвращаем валидный JSON после двух "сбоев"
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

        // Ретрай на 500 и т.п.
        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(10));

        var policyHandler = new PolicyHttpMessageHandler(retryPolicy)
        {
            InnerHandler = testHandler
        };

        var httpClient = new HttpClient(policyHandler)
        {
            BaseAddress = new Uri("https://fake-flight-provider.wiremockapi.cloud")
        };

        // Мокаем маппер: просто возвращаем фиктивный список из одного Flight
        _mapperMock
            .Setup(m => m.Map<IEnumerable<Flight>>(It.IsAny<IEnumerable<object>>()))
            .Returns(new List<Flight>
            {
                new Flight(
                    flightNumber: "AB123",
                    departureTime: DateTime.UtcNow.AddHours(1),
                    arrivalTime: DateTime.UtcNow.AddHours(2),
                    durationMinutes: 120,
                    airline: new Airline("Test Airline"),
                    price: new Money(100),
                    stops: 0,
                    stopDetails: new List<Flight.StopDetailData>(),
                    origin: "Москва",
                    destination: "Санкт-Петербург",
                    source: "source1"
                )
            });

        var provider = new FakeFlightProviderA(httpClient, _mapperMock.Object, _loggerMock.Object);
        var query = new SearchFlightsQuery(FlightNumber: null);

        // Act
        var flights = await provider.GetFlightsAsync(query, CancellationToken.None);

        // Assert
        var flightList = flights.ToList();
        Assert.Single(flightList);
        Assert.Equal("AB123", flightList[0].FlightNumber);
        Assert.Equal(3, testHandler.CallCount); // 2 фейла + 1 успех
    }
}