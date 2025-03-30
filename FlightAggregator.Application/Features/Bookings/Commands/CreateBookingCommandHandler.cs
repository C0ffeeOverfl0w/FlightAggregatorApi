namespace FlightAggregator.Application.Features.Bookings.Commands;

/// <summary>
/// Обработчик команды создания бронирования.
/// </summary>
public sealed class CreateBookingCommandHandler(
    IFlightProvider flightProvider,
    BookingDomainService bookingService,
    IBookingRepository bookingRepository,
    ILogger<CreateBookingCommandHandler> logger) : IRequestHandler<CreateBookingCommand, BookingResponse>
{
    private readonly IFlightProvider _flightProvider = flightProvider;
    private readonly BookingDomainService _bookingService = bookingService;
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly ILogger<CreateBookingCommandHandler> _logger = logger;

    public async Task<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString();
        _logger.LogInformation("Обработка команды бронирования с ID {RequestId}: {@Request}", requestId, request);

        var query = new SearchFlightsQuery(request.FlightNumber);
        var flights = await _flightProvider.GetFlightsAsync(query, cancellationToken);

        var flight = flights.FirstOrDefault();
        if (flight == null)
        {
            throw new ArgumentException($"Рейс с номером {request.FlightNumber} не найден.");
        }

        var booking = _bookingService.CreateBooking(flight, request.PassengerName, request.PassengerEmail);

        var bookingRequest = new BookingRequest(flight.FlightNumber, request.PassengerName);
        var providerResponse = await _flightProvider.BookFlightAsync(bookingRequest, cancellationToken);

        if (!providerResponse.IsSuccess)
        {
            throw new ArgumentException(providerResponse.Message);
        }

        await _bookingRepository.AddAsync(booking, cancellationToken);

        _logger.LogInformation("Бронирование {BookingId} успешно создано", booking.Id);

        return new BookingResponse(
            booking.Id.ToString(),
            request.FlightNumber,
            request.PassengerName,
            booking.BookingDate,
            true,
            "Бронирование успешно создано.");
    }
}