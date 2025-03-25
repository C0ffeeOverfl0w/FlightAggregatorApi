namespace FlightAggregator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BookingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingController> _logger;

    public BookingController(IMediator mediator, ILogger<BookingController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Создание бронирования для выбранного рейса.
    /// </summary>
    /// <param name="request">Данные бронирования.</param>
    /// <param name="cancellationToken">Токен отмены запроса.</param>
    /// <returns>Подтверждение бронирования.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateBooking(
        [FromBody] CreateBookingCommand request,
        CancellationToken cancellationToken)
    {
        // Строковая интерполяция допустима, но рекомендуется использовать шаблоны сообщений для сохранения структурированного логирования.
        _logger.LogInformation("Получен запрос на создание бронирования для рейса: {FlightNumber}", request.FlightNumber);
        var result = await _mediator.Send(request, cancellationToken);
        _logger.LogInformation("Бронирование успешно создано для рейса: {FlightNumber}", request.FlightNumber);
        return Ok(result);
    }
}