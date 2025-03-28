namespace FlightAggregator.Application.Features.Bookings.Commands;

/// <summary>
/// Команда для создания бронирования рейса.
/// </summary>
public record CreateBookingCommand(
    string FlightNumber,
    string PassengerName,
    string PassengerEmail
) : IRequest<BookingResponse>;
