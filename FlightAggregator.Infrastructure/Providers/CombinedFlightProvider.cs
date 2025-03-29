namespace FlightAggregator.Infrastructure.Providers;

/// <inheritdoc/>
public class CombinedFlightProvider(IEnumerable<IFlightProvider> providers, ILogger logger) : IFlightProvider
{
    private readonly IEnumerable<IFlightProvider> _providers = providers;
    private readonly ILogger _logger = logger;

    /// <inheritdoc/>
    public async Task<IEnumerable<Flight>> GetFlightsAsync(SearchFlightsQuery query, CancellationToken cancellationToken)
    {
        var tasks = _providers.Select(s => s.GetFlightsAsync(query, cancellationToken));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r);
    }

    /// <inheritdoc/>
    public async Task<ProviderBookingResponse> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
    {
        foreach (var strategy in _providers)
        {
            try
            {
                var response = await strategy.BookFlightAsync(request, cancellationToken);
                if (response.IsSuccess)
                    return response;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не удалось забронировать рейс {FlightNumber} у провайдера {ProviderName}.",
                    request.FlightNumber, strategy.GetType().Name);
            }
        }
        return new ProviderBookingResponse(
            BookingId: null,
            FlightNumber: request.FlightNumber,
            PassengerName: request.PassengerName,
            BookingDate: null,
            IsSuccess: false,
            Message: "Не удалось забронировать рейс ни у одного провайдера."
        );
    }

    /// <inheritdoc/>
    public async Task<ProviderBookingResponse> CancelBookingAsync(string bookingId, CancellationToken cancellationToken)
    {
        foreach (var strategy in _providers)
        {
            try
            {
                var response = await strategy.CancelBookingAsync(bookingId, cancellationToken);
                if (response.IsSuccess)
                    return response;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не удалось отменить бронирование {BookingId} у провайдера {ProviderName}.",
                    bookingId, strategy.GetType().Name);
            }
        }
        return new ProviderBookingResponse(
            BookingId: null,
            FlightNumber: string.Empty,
            PassengerName: string.Empty,
            BookingDate: null,
            IsSuccess: false,
            Message: "Не удалось отменить бронирование ни у одного провайдера."
        );
    }
}