namespace FlightAggregator.Application.Features.Bookings.Commands;

/// <summary>
/// Отмена бронирования.
/// </summary>
/// <param name="BookingId">Идентификатор брони</param>
public record CancelBookingCommand(string BookingId) : IRequest<BookingResponse>;
