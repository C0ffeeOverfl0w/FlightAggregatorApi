using static FlightAggregator.Application.Interfaces.IFlightProvider;

namespace FlightAggregator.Infrastructure.Providers;

/// <summary>
/// Базовый класс для фейкового провайдера данных о рейсах.
/// </summary>
public abstract class BaseFakeFlightProvider(HttpClient httpClient) : IFlightProvider
{
    protected HttpClient HttpClient { get; } = httpClient;

    /// <inheritdoc/>
    public async Task<IEnumerable<Flight>> GetFlightsAsync(
        string? flightNumber,
        string origin,
        string destination,
        DateTime? departureTime,
        DateTime? arrivalTime,
        string? airline,
        decimal? maxPrice,
        int? maxStops,
        int passengers,
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
                var airlineObj = new Airline(Guid.NewGuid(), fd.Airline);
                var stopDetails = fd.StopDetails != null
                    ? fd.StopDetails.Select(sd => new Flight.StopDetailData(sd.Airport, sd.DurationMinutes)).ToList()
                    : new List<Flight.StopDetailData>();

                return new Flight(
                    id: fd.Id,
                    flightNumber: fd.FlightNumber,
                    departureTime: fd.DepartureTime,
                    arrivalTime: fd.ArrivalTime,
                    durationMinutes: fd.DurationMinutes,
                    airline: airlineObj,
                    price: new Money(fd.Price, "USD"),
                    stops: fd.Stops,
                    stopDetails: stopDetails,
                    origin: fd.Origin,
                    destination: fd.Destination,
                    source: fd.Source
                );
            });

            // Применяем фильтрацию через компоновку предикатов
            Func<Flight, bool> filter = f => true;

            // Фильтр по городу вылета
            if (!string.IsNullOrWhiteSpace(origin))
                filter = filter.And(f => f.Origin.Contains(origin, StringComparison.OrdinalIgnoreCase));

            // Фильтр по городу прилёта
            if (!string.IsNullOrWhiteSpace(destination))
                filter = filter.And(f => f.Destination.Contains(destination, StringComparison.OrdinalIgnoreCase));

            // Фильтр по дате вылета
            if (departureTime.HasValue)
                filter = filter.And(f => f.DepartureTime.Date == departureTime.Value.Date);

            // Фильтр по дате возвращения
            if (arrivalTime.HasValue)
                filter = filter.And(f => f.ArrivalTime.Date == arrivalTime.Value.Date);

            // Фильтр по авиакомпании
            if (!string.IsNullOrWhiteSpace(airline))
                filter = filter.And(f => f.Airline.Name.Equals(airline, StringComparison.OrdinalIgnoreCase));

            // Фильтр по цене
            if (maxPrice.HasValue)
                filter = filter.And(f => f.Price.Amount <= maxPrice.Value);

            // Фильтр по количеству пересадок
            if (maxStops.HasValue)
                filter = filter.And(f => f.Stops <= maxStops.Value);

            // Фильтр по номеру рейса, если указан
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

    public async Task<BookingResponse> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"/book?flightNumber={Uri.EscapeDataString(request.FlightNumber)}&passengerName={Uri.EscapeDataString(request.PassengerName)}";
            var response = await HttpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();

            return new BookingResponse(Guid.NewGuid().ToString(), request.FlightNumber, request.PassengerName, DateTime.UtcNow, true, "Бронирование успешно");
        }
        catch (Exception ex)
        {
            return new BookingResponse(null, request.FlightNumber, request.PassengerName, null, false, $"Ошибка при бронировании: {ex.Message}");
        }
    }

    public Task<BookingResponse> CancelBookingAsync(string bookingId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private sealed record FlightData(
        string Id,
        string Airline,
        string FlightNumber,
        string Origin,
        string Destination,
        DateTime DepartureTime,
        DateTime ArrivalTime,
        int DurationMinutes,
        decimal Price,
        int Stops,
        List<StopDetailData>? StopDetails,
        string Source
    );

    private sealed record StopDetailData(
    string Airport,
    int DurationMinutes
);
}