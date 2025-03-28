namespace FlightAggregator.Application.Features.Flights.Queries;

/// <summary>
/// Обработчик запроса поиска авиарейсов, который использует композитный провайдер для агрегирования данных из нескольких источников,
/// применяет сортировку и возвращает список FlightDto.
/// </summary>
public sealed class SearchFlightsQueryHandler : IRequestHandler<SearchFlightsQuery, IEnumerable<FlightDto>>
{
    private readonly IFlightProvider _compositeProvider;

    public SearchFlightsQueryHandler(IFlightProvider compositeProvider, IValidator<SearchFlightsQuery> @object) =>
        _compositeProvider = compositeProvider;

    public async Task<IEnumerable<FlightDto>> Handle(SearchFlightsQuery query, CancellationToken cancellationToken)
    {
        var flights = await _compositeProvider.GetFlightsAsync(
            query.FlightNumber,
            query.Origin,
            query.Destination,
            query.DepartureTime,
            query.ArrivalTime,
            query.Airline,
            query.MaxPrice,
            query.MaxStops,
            query.Passengers,
            query.PageNumber,
            query.PageSize,

            cancellationToken);

        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            flights = query.SortBy.ToLower() switch
            {
                "price" => query.SortOrder?.ToLower() == "desc"
                    ? flights.OrderByDescending(f => f.Price.Amount)
                    : flights.OrderBy(f => f.Price.Amount),
                "date" => query.SortOrder?.ToLower() == "desc"
                    ? flights.OrderByDescending(f => f.DepartureTime)
                    : flights.OrderBy(f => f.DepartureTime),
                _ => flights
            };
        }

        // Преобразуем доменные объекты Flight в DTO
        var flightDtos = flights.Select(f => new FlightDto(
            f.FlightNumber,
            f.DepartureTime,
            f.ArrivalTime,
            f.Origin,
            f.Destination,
            f.DurationMinutes,
            f.StopDetails.Select(sd => new StopDetailData(sd.Airport, sd.DurationMinutes)).ToList(),
            f.Airline.Name,
            f.Price.Amount,
            f.Stops
        ));

        return flightDtos;
    }
}