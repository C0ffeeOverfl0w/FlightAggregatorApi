namespace FlightAggregator.Infrastructure.Repositories;

/// <inheritdoc/>
public class InMemoryBookingRepository : IBookingRepository
{
    private readonly ConcurrentDictionary<string, Booking> _bookings = new();

    /// <inheritdoc/>
    public Task<Booking> GetByIdAsync(string bookingId, CancellationToken cancellationToken)
    {
        _bookings.TryGetValue(bookingId, out var booking);
        return Task.FromResult(booking);
    }

    /// <inheritdoc/>
    public Task AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        _bookings[booking.Id.ToString()] = booking;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Booking booking, CancellationToken cancellationToken)
    {
        _bookings[booking.Id.ToString()] = booking;
        return Task.CompletedTask;
    }
}