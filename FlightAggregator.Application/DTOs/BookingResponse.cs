namespace FlightAggregator.Application.DTOs;

/// <summary>
/// Модель ответа после успешного бронирования.
/// </summary>
public record BookingResponse(
    string? BookingId,
    string FlightNumber,
    string PassengerName,
    DateTime? BookingDate,
    bool IsSuccess,
    string? Message);