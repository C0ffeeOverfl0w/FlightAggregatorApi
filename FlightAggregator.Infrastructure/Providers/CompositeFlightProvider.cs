using static FlightAggregator.Application.Interfaces.IFlightProvider;

namespace FlightAggregator.Infrastructure.Providers;

/// <inheritdoc/>
public sealed class CompositeFlightProvider : IFlightProvider
{
    private readonly IEnumerable<IFlightProvider> _providers;

    public CompositeFlightProvider(IEnumerable<IFlightProvider> providers) =>
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));

    /// <inheritdoc/>
    public async Task<IEnumerable<Flight>> GetFlightsAsync(
        string? flightNumber,
        string origin,
        string destination,
        DateTime? departureDate,
        DateTime? returnDate,
        string? airline,
        decimal? maxPrice,
        int? maxStops,
        int passengers,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        // Запускаем параллельно запросы ко всем провайдерам
        var tasks = _providers.Select(provider => provider.GetFlightsAsync(
            flightNumber,
            origin,
            destination,
            departureDate,
            returnDate,
            airline,
            maxPrice,
            maxStops,
            passengers,
            pageNumber,
            pageSize,
            cancellationToken));

        var results = await Task.WhenAll(tasks);

        return results.SelectMany(r => r);
    }

    public async Task<BookingResponse> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
    {
        var flightsTasks = _providers.Select(p => p.GetFlightsAsync(
            request.FlightNumber, null, null, null, null, null, null, null, 1, 1, 1, cancellationToken));
        var flightsResults = await Task.WhenAll(flightsTasks);
        var provider = _providers.FirstOrDefault(p => flightsResults.Any(f => f.Any(flight => flight.FlightNumber == request.FlightNumber)));

        if (provider == null)
        {
            return new BookingResponse(null, request.FlightNumber, request.PassengerName, null, false, "Провайдер для данного рейса не найден.");
        }

        return await provider.BookFlightAsync(request, cancellationToken);
    }

    public Task<BookingResponse> CancelBookingAsync(string bookingId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}