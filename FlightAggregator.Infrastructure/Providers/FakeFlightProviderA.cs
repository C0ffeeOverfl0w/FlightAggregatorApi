﻿namespace FlightAggregator.Infrastructure.Providers;

/// <inheritdoc/>
public class FakeFlightProviderA(
    HttpClient httpClient,
    IMapper mapper,
    ILogger<FakeFlightProviderA> logger) : IFlightProvider
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<FakeFlightProviderA> _logger = logger;

    /// <inheritdoc/>
    public async Task<IEnumerable<Flight>> GetFlightsAsync(SearchFlightsQuery query, CancellationToken cancellationToken)
    {
        var url = $"/flights";

        try
        {
            _logger.LogInformation("Запрос рейсов у ProviderA: {Url}", url);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var flightsData = JsonSerializer.Deserialize<IEnumerable<FlightDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? Enumerable.Empty<FlightDto>();

            _logger.LogInformation("ProviderA вернул {Count} рейсов", flightsData.Count());

            return _mapper.Map<IEnumerable<Flight>>(flightsData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении рейсов от ProviderA");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<ProviderBookingResponse> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
    {
        var url = $"/book";

        try
        {
            _logger.LogInformation("Попытка бронирования рейса {FlightNumber} в ProviderA", request.FlightNumber);

            var response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();

            var bookingResponse = new ProviderBookingResponse(
                BookingId: Guid.NewGuid().ToString(),
                FlightNumber: request.FlightNumber,
                PassengerName: request.PassengerName,
                BookingDate: DateTime.UtcNow,
                IsSuccess: true,
                Message: "Бронирование успешно выполнено"
            );

            _logger.LogInformation("Бронирование рейса {FlightNumber} в ProviderA успешно: {BookingId}", request.FlightNumber, bookingResponse.BookingId);

            return bookingResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при бронировании рейса {FlightNumber} в ProviderA", request.FlightNumber);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<ProviderBookingResponse> CancelBookingAsync(string bookingId, CancellationToken cancellationToken)
    {
        var url = $"/cancelBooking";
        try
        {
            _logger.LogInformation("Попытка отмены бронирования {BookingId} в ProviderA", bookingId);

            var response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();

            var cancelResponse = new ProviderBookingResponse(
                BookingId: bookingId,
                FlightNumber: string.Empty,
                PassengerName: string.Empty,
                BookingDate: DateTime.UtcNow,
                IsSuccess: true,
                Message: "Отмена бронирования успешна"
            );

            _logger.LogInformation("Отмена бронирования {BookingId} в ProviderA успешна", bookingId);

            return cancelResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отмене бронирования {BookingId} в ProviderA", bookingId);
            throw;
        }
    }
}