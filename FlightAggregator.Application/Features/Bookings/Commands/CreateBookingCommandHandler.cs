namespace FlightAggregator.Application.Features.Bookings.Commands;

/// <summary>
/// Обработчик команды создания бронирования.
/// </summary>
public sealed class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingResponse>
{
    public Task<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken) =>
        Task.FromResult(new BookingResponse(
            BookingId: Guid.NewGuid().ToString(),
            FlightNumber: request.FlightNumber,
            PassengerName: request.PassengerName,
            BookingDate: DateTime.UtcNow));
}