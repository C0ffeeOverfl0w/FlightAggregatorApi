using Microsoft.AspNetCore.Authorization;

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
    [Authorize]
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

    /// <summary>
    /// Отмена бронирования по его идентификатору.
    /// </summary>
    /// <param name="bookingId">Идентификатор бронирования.</param>
    /// <param name="cancellationToken">Токен отмены запроса.</param>
    /// <returns>Результат отмены бронирования.</returns>
    [HttpDelete("{bookingId}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelBooking(string bookingId, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString();
        _logger.LogInformation("Получен запрос на отмену бронирования с ID {BookingId} и RequestId {RequestId}", bookingId, requestId);

        var command = new CancelBookingCommand(bookingId);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Не удалось отменить бронирование с ID {BookingId} для RequestId {RequestId}: {Message}",
                bookingId, requestId, result.Message);
            return BadRequest(result.Message);
        }

        _logger.LogInformation("Бронирование с ID {BookingId} успешно отменено для RequestId {RequestId}", bookingId, requestId);
        return Ok(result);
    }
}