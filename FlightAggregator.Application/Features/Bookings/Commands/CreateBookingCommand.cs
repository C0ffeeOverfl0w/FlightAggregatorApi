namespace FlightAggregator.Application.Features.Bookings.Commands;

/// <summary>
/// Команда для создания бронирования рейса.
/// </summary>
public record CreateBookingCommand(
    string FlightNumber,
    string PassengerName,
    string PassengerEmail
) : IRequest<BookingResponse>;
/// <summary>
/// Отмена бронирования.
/// </summary>
/// <param name="BookingId">Идентификатор брони</param>
public record CancelBookingCommand(string BookingId) : IRequest<BookingResponse>;