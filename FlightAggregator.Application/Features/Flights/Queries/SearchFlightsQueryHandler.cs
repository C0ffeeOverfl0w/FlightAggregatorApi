namespace FlightAggregator.Application.Features.Flights.Queries;

/// <summary>
/// Обработчик запроса поиска авиарейсов, который агрегирует данные от нескольких провайдеров,
/// применяет фильтрацию, сортировку и пагинацию.
/// </summary>
public sealed class SearchFlightsQueryHandler : IRequestHandler<SearchFlightsQuery, IEnumerable<FlightDto>>
{
    private readonly IFlightProviderFactory _providerFactory;

    public SearchFlightsQueryHandler(IFlightProviderFactory providerFactory) =>
        _providerFactory = providerFactory;

    public async Task<IEnumerable<FlightDto>> Handle(SearchFlightsQuery query, CancellationToken cancellationToken)
    {
        // Получаем провайдеры через фабрику
        var providerA = _providerFactory.GetFlightProvider(FlightProviderType.ProviderA);
        var providerB = _providerFactory.GetFlightProvider(FlightProviderType.ProviderB);

        // Параллельно получаем списки рейсов от каждого провайдера
        var flightsFromA = await providerA.GetFlightsAsync(
            query.FlightNumber,
            query.DepartureDate,
            query.Airline,
            query.MaxPrice,
            query.MaxStops,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        var flightsFromB = await providerB.GetFlightsAsync(
            query.FlightNumber,
            query.DepartureDate,
            query.Airline,
            query.MaxPrice,
            query.MaxStops,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        var allFlights = flightsFromA.Concat(flightsFromB);

        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            allFlights = query.SortBy.ToLower() switch
            {
                "price" => query.SortOrder?.ToLower() == "desc"
                    ? allFlights.OrderByDescending(f => f.Price.Amount)
                    : allFlights.OrderBy(f => f.Price.Amount),
                "date" => query.SortOrder?.ToLower() == "desc"
                    ? allFlights.OrderByDescending(f => f.DepartureDate)
                    : allFlights.OrderBy(f => f.DepartureDate),
                _ => allFlights
            };
        }

        var flightDtos = allFlights.Select(f => new FlightDto(
            f.FlightNumber,
            f.DepartureDate,
            f.Airline.Name,
            f.Price.Amount,
            f.Stops
        ));

        // flightDtos = flightDtos.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);

        return flightDtos;
    }
}