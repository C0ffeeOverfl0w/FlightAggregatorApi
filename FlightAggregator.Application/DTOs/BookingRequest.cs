namespace FlightAggregator.Application.DTOs;

/// <summary>
/// Запрос на бронирование рейса.
/// </summary>
public record BookingRequest(string FlightNumber, string PassengerName);