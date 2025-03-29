namespace FlightAggregator.Application.Features.Flights.Queries;

/// <summary>
/// Обработчик запроса поиска авиарейсов, который использует
/// композитный провайдер для агрегирования данных из нескольких источников.
/// </summary>
public sealed class SearchFlightsQueryHandler(
    IFlightProvider provider,
    IMapper mapper,
    IFlightFilterService filterService,
    IFlightSortService sortService,
    IValidator<SearchFlightsQuery> @object) : IRequestHandler<SearchFlightsQuery, IEnumerable<FlightDto>>
{
    private readonly IFlightProvider _provider = provider;
    private readonly IMapper _mapper = mapper;
    private readonly IFlightFilterService _filterService = filterService;
    private readonly IFlightSortService _sortService = sortService;

    /// <summary>
    /// Обрабатывает запрос на поиск авиарейсов.
    /// </summary>
    /// <param name="query">Запрос на поиск рейсов.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция DTO объектов найденных рейсов.</returns>
    public async Task<IEnumerable<FlightDto>> Handle(SearchFlightsQuery query, CancellationToken cancellationToken)
    {
        var flights = await _provider.GetFlightsAsync(query, cancellationToken);

        var filteredFlights = _filterService.ApplyFilters(flights, query);

        var sortedFlights = _sortService.Sort(filteredFlights, query);

        var flightDtos = _mapper.Map<IEnumerable<FlightDto>>(sortedFlights);

        return flightDtos;
    }
}