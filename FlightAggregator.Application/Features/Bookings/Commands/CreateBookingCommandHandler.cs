namespace FlightAggregator.Application.Features.Bookings.Commands;

/// <summary>
/// Обработчик команды создания бронирования.
/// </summary>
public sealed class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingResponse>
{
    private readonly IFlightProvider _flightProvider;
    private readonly BookingDomainService _bookingService;
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<CreateBookingCommandHandler> _logger;

    public CreateBookingCommandHandler(
        IFlightProvider flightProvider,
        BookingDomainService bookingService,
        IBookingRepository bookingRepository,
        ILogger<CreateBookingCommandHandler> logger)
    {
        _flightProvider = flightProvider;
        _bookingService = bookingService;
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString();
        _logger.LogInformation("Начало обработки команды бронирования с ID {RequestId}: {@Request}", requestId, request);

        // Ищем рейс
        var flights = await _flightProvider.GetFlightsAsync(
            flightNumber: request.FlightNumber,
            origin: "",
            destination: "",
            departureDate: null,
            returnDate: null,
            airline: null,
            maxPrice: null,
            maxStops: null,
            passengers: 1,
            pageNumber: 1,
            pageSize: 1,
            cancellationToken: cancellationToken);

        var flight = flights.FirstOrDefault();
        if (flight == null)
        {
            _logger.LogWarning("Рейс с номером {FlightNumber} не найден для запроса с ID {RequestId}", request.FlightNumber, requestId);
            return new BookingResponse(null, request.FlightNumber, request.PassengerName, null, false, "Рейс не найден.");
        }

        try
        {
            // Создаем бронирование через доменный сервис
            var booking = _bookingService.CreateBooking(flight, request.PassengerName, request.PassengerEmail);

            // Отправляем запрос на бронирование в источник
            var bookingRequest = new IFlightProvider.BookingRequest(flight.FlightNumber, request.PassengerName);
            var providerResponse = await _flightProvider.BookFlightAsync(bookingRequest, cancellationToken);

            if (!providerResponse.IsSuccess)
            {
                _logger.LogWarning("Не удалось забронировать рейс {FlightNumber} для запроса с ID {RequestId}: {Message}",
                    request.FlightNumber, requestId, providerResponse.Message);
                return new BookingResponse(null, request.FlightNumber, request.PassengerName, null, false, providerResponse.Message);
            }

            // Сохраняем бронирование
            await _bookingRepository.AddAsync(booking, cancellationToken);

            _logger.LogInformation("Бронирование {BookingId} успешно создано для рейса {FlightNumber} для запроса с ID {RequestId}",
                booking.Id, request.FlightNumber, requestId);

            return new BookingResponse(
                BookingId: booking.Id.ToString(),
                FlightNumber: request.FlightNumber,
                PassengerName: request.PassengerName,
                BookingDate: booking.BookingDate,
                IsSuccess: true,
                Message: "Бронирование успешно создано.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании бронирования для рейса {FlightNumber} для запроса с ID {RequestId}",
                request.FlightNumber, requestId);
            return new BookingResponse(null, request.FlightNumber, request.PassengerName, null, false, ex.Message);
        }
    }
}