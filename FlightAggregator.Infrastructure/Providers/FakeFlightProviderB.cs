namespace FlightAggregator.Infrastructure.Providers;

/// <inheritdoc/>
public class FakeFlightProviderB(HttpClient httpClient, IMapper mapper) : IFlightProvider
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc/>
    public async Task<IEnumerable<Flight>> GetFlightsAsync(SearchFlightsQuery query, CancellationToken cancellationToken)
    {
        var url = $"/things";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var flightsData = JsonSerializer.Deserialize<IEnumerable<FlightDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                          ?? Enumerable.Empty<FlightDto>();

        return _mapper.Map<IEnumerable<Flight>>(flightsData);
    }

    /// <inheritdoc/>
    public async Task<ProviderBookingResponse> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
    {
        var url = $"/book";
        var response = await _httpClient.PostAsync(url, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return new ProviderBookingResponse(
            BookingId: Guid.NewGuid().ToString(),
            FlightNumber: request.FlightNumber,
            PassengerName: request.PassengerName,
            BookingDate: DateTime.UtcNow,
            IsSuccess: true,
            Message: "Бронирование успешно выполнено"
        );
    }

    /// <inheritdoc/>
    public async Task<ProviderBookingResponse> CancelBookingAsync(string bookingId, CancellationToken cancellationToken)
    {
        var url = $"/cancelBooking";
        var response = await _httpClient.PostAsync(url, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return new ProviderBookingResponse(
            BookingId: bookingId,
            FlightNumber: string.Empty,
            PassengerName: string.Empty,
            BookingDate: DateTime.UtcNow,
            IsSuccess: true,
            Message: "Отмена бронирования успешна"
        );
    }
}