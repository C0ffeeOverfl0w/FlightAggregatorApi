namespace FlightAggregator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FlightController> _logger;

    public FlightController(IMediator mediator, ILogger<FlightController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Поиск авиарейсов по заданным критериям.
    /// </summary>
    /// <param name="departureDate">Дата вылета.</param>
    /// <param name="airline">Авиакомпания (опционально).</param>
    /// <param name="maxPrice">Максимальная цена (опционально).</param>
    /// <param name="maxStops">Максимальное количество пересадок (опционально).</param>
    /// <param name="cancellationToken">Токен отмены запроса.</param>
    /// <returns>Список найденных рейсов.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FlightDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFlights(
        [FromQuery] DateTime departureDate,
        [FromQuery] string? airline,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? maxStops,
        CancellationToken cancellationToken)
    {
        if (departureDate == default)
        {
            _logger.LogWarning("Неверная дата вылета.");
            return BadRequest("Необходимо указать корректную дату вылета.");
        }

        try
        {
            var query = new SearchFlightsQuery(departureDate, airline, maxPrice, maxStops);
            IEnumerable<FlightDto> flights = await _mediator.Send(query, cancellationToken);
            var flightsList = flights.ToList();

            _logger.LogInformation("Найдено {Count} авиарейсов для даты {DepartureDate} с: airline={Airline}, maxPrice={MaxPrice}, maxStops={MaxStops}",
                flightsList.Count, departureDate, airline, maxPrice, maxStops);

            return Ok(flightsList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при поиске авиарейсов");
            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка при поиске авиарейсов");
        }
    }
}