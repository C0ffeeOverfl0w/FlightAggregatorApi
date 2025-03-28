using static FlightAggregator.Application.Interfaces.IFlightProvider;

namespace FlightAggregator.Infrastructure.Caching
{
    public class CachingFlightProvider : IFlightProvider
    {
        private readonly IFlightProvider _inner;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public CachingFlightProvider(IFlightProvider inner, IMemoryCache cache)
        {
            _inner = inner;
            _cache = cache;
        }

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
            var cacheKey = GenerateCacheKey(flightNumber, origin, destination, departureTime, arrivalTime, airline, maxPrice, maxStops, passengers, pageNumber, pageSize);

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Flight>? cachedFlights))
            {
                return cachedFlights ?? Enumerable.Empty<Flight>();
            }

            var flights = await _inner.GetFlightsAsync(
                flightNumber, origin, destination, departureTime, arrivalTime, airline, maxPrice, maxStops, passengers, pageNumber, pageSize, cancellationToken);

            _cache.Set(cacheKey, flights, _cacheDuration);

            return flights;
        }

        public async Task<BookingResponse> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
        {
            return await _inner.BookFlightAsync(request, cancellationToken);
        }

        public Task<BookingResponse> CancelBookingAsync(string bookingId, CancellationToken cancellationToken)
        {
            return _inner.CancelBookingAsync(bookingId, cancellationToken);
        }

        private string GenerateCacheKey(
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
            int pageSize)
        {
            return $"flightNumber={flightNumber};origin={origin};destination={destination};departure={departureTime};arrival={arrivalTime};airline={airline};maxPrice={maxPrice};maxStops={maxStops};passengers={passengers};pageNumber={pageNumber};pageSize={pageSize}";
        }
    }
}