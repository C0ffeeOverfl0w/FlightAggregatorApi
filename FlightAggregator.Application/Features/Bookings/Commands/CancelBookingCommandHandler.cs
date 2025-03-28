
namespace FlightAggregator.Application.Features.Bookings.Commands;

/// <summary>
/// Обработчик команды отмены бронирования.
/// </summary>
public sealed class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, BookingResponse>
{
    private readonly IFlightProvider _flightProvider;
    private readonly ILogger<CancelBookingCommandHandler> _logger;

    public CancelBookingCommandHandler(IFlightProvider flightProvider, ILogger<CancelBookingCommandHandler> logger)
    {
        _flightProvider = flightProvider;
        _logger = logger;
    }

    public async Task<BookingResponse> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Начало отмены бронирования с ID {BookingId}", request.BookingId);
        try
        {
            var response = await _flightProvider.CancelBookingAsync(request.BookingId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation("Отмена бронирования с ID {BookingId} успешна", request.BookingId);
            }
            else
            {
                _logger.LogWarning("Отмена бронирования с ID {BookingId} завершилась с ошибкой: {Message}", request.BookingId, response.Message);
            }
            return new BookingResponse(
                response.BookingId, response.FlightNumber, response.PassengerName, response.BookingDate,
                response.IsSuccess, response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отмене бронирования с ID {BookingId}", request.BookingId);
            return new BookingResponse(null, string.Empty, string.Empty, null, false, ex.Message);
        }
    }
}
