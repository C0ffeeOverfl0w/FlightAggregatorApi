namespace FlightAggregator.Infrastructure.Providers;

/// <summary>
/// Базовый класс для фейкового провайдера данных о рейсах.
/// </summary>
public abstract class BaseFakeFlightProvider(HttpClient httpClient) : IFlightProvider
{
    protected HttpClient HttpClient { get; } = httpClient;

    public async Task<IEnumerable<Flight>> GetFlightsAsync(
        string? flightNumber,
        DateTime? departureDate,
        string? airline,
        decimal? maxPrice,
        int? maxStops,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        try
        {
            var url = "/flights";

            // Выполняем HTTP-запрос к WireMock Cloud stub
            var response = await HttpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var allFlightData = JsonSerializer.Deserialize<IEnumerable<FlightData>>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new Exception("Не удалось десериализовать данные о рейсах.");

            // Преобразуем в доменные объекты
            var flights = allFlightData.Select(fd =>
            {
                var airlineObj = new Airline(Guid.NewGuid(), fd.AirlineName);
                return new Flight(fd.FlightNumber, fd.DepartureDate, airlineObj, new Money(fd.Price, "USD"), fd.Stops);
            });

            // Применяем фильтрацию через компоновку предикатов
            Func<Flight, bool> filter = f => true;

            if (departureDate.HasValue)
                filter = filter.And(f => f.DepartureDate.Date == departureDate.Value.Date);
            if (!string.IsNullOrWhiteSpace(airline))
                filter = filter.And(f => f.Airline.Name.Equals(airline, StringComparison.OrdinalIgnoreCase));
            if (maxPrice.HasValue)
                filter = filter.And(f => f.Price.Amount <= maxPrice.Value);
            if (maxStops.HasValue)
                filter = filter.And(f => f.Stops <= maxStops.Value);
            if (!string.IsNullOrWhiteSpace(flightNumber))
                filter = filter.And(f => f.FlightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));

            var filteredFlights = flights.Where(filter);

            var pagedFlights = filteredFlights
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return pagedFlights;
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при получении списка рейсов: " + ex.Message, ex);
        }
    }

    private sealed record FlightData(
        string FlightNumber,
        DateTime DepartureDate,
        string AirlineName,
        decimal Price,
        int Stops
    );
}