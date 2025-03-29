namespace FlightAggregator.Infrastructure.Caching;

/// <inheritdoc/>
public class CachingFlightProvider(IFlightProvider inner, IMemoryCache cache) : IFlightProvider
{
    private readonly IFlightProvider _inner = inner;
    private readonly IMemoryCache _cache = cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    /// <inheritdoc/>
    public async Task<IEnumerable<Flight>> GetFlightsAsync(SearchFlightsQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = GenerateCacheKey(query);
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Flight>? cachedFlights) && cachedFlights != null)
            return cachedFlights;

        var flights = await _inner.GetFlightsAsync(query, cancellationToken);
        _cache.Set(cacheKey, flights, _cacheDuration);
        return flights;
    }

    /// <inheritdoc/>
    public Task<ProviderBookingResponse> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
        => _inner.BookFlightAsync(request, cancellationToken);

    public Task<ProviderBookingResponse> CancelBookingAsync(string bookingId, CancellationToken cancellationToken)
        => _inner.CancelBookingAsync(bookingId, cancellationToken);

    /// <summary>
    /// Генерация ключа для кэширования.
    /// </summary>
    /// <param name="query">Параметры запроса</param>
    /// <returns>Ключ</returns>
    private string GenerateCacheKey(SearchFlightsQuery query)
    {
        return $"Flights:{query.Origin}:{query.Destination}:" +
            $"{query.DepartureTime}:{query.ArrivalTime}:" +
            $"{query.Airline}:{query.MaxPrice}:{query.MaxStops}:" +
            $"{query.Passengers}:{query.PageNumber}:{query.PageSize}";
    }
}