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
    /// <param name="origin">Город вылета (обязательный).</param>
    /// <param name="destination">Город прилёта (обязательный).</param>
    /// <param name="departureDate">Дата вылета (обязательная).</param>
    /// <param name="returnDate">Дата возвращения (необязательная).</param>
    /// <param name="airline">Авиакомпания (необязательно).</param>
    /// <param name="maxPrice">Максимальная цена (необязательно).</param>
    /// <param name="maxStops">Максимальное количество пересадок (необязательно).</param>
    /// <param name="passengers">Количество пассажиров (необязательно, по умолчанию 1).</param>
    /// <param name="sortBy">Поле для сортировки (например, price или date).</param>
    /// <param name="sortOrder">Порядок сортировки (asc или desc).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список найденных рейсов.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FlightDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFlights(
        [FromQuery] string origin,
        [FromQuery] string destination,
        [FromQuery] DateTime departureTime,
        [FromQuery] DateTime? arrivalTime,
        [FromQuery] string? airline,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? maxStops,
        [FromQuery] int passengers = 1,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchFlightsQuery(
            FlightNumber: null,
            Origin: origin,
            Destination: destination,
            DepartureTime: departureTime,
            ArrivalTime: arrivalTime,
            Airline: airline,
            MaxPrice: maxPrice,
            MaxStops: maxStops,
            Passengers: passengers,
            SortBy: sortBy,
            SortOrder: sortOrder,
            PageNumber: pageNumber,
            PageSize: pageSize
        );

        var flights = await _mediator.Send(query, cancellationToken);
        var flightsList = flights.ToList();

        _logger.LogInformation(
            "Найдено {Count} авиарейсов из {Origin} в {Destination} на {DepartureDate} (return={ReturnDate}), airline={Airline}, maxPrice={MaxPrice}, maxStops={MaxStops}, passengers={Passengers}",
            flightsList.Count, origin, destination, departureTime, arrivalTime, airline, maxPrice, maxStops, passengers
        );

        return Ok(flightsList);
    }
}