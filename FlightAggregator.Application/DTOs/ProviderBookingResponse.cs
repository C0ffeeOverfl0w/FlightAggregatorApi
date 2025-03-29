namespace FlightAggregator.Application.DTOs;

/// <summary>
/// Представляет ответ бронирования от провайдера рейсов.
/// </summary>
public record ProviderBookingResponse(
    string? BookingId,
    string FlightNumber,
    string PassengerName,
    DateTime? BookingDate,
    bool IsSuccess,
    string? Message);