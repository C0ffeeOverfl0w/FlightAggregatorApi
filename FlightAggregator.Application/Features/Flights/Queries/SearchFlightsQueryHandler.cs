namespace FlightAggregator.Application.Features.Flights.Queries;

/// <summary>
/// Обработчик запроса поиска авиарейсов.
/// </summary>
public sealed class SearchFlightsQueryHandler : IRequestHandler<SearchFlightsQuery, IEnumerable<FlightDto>>
{
    public Task<IEnumerable<FlightDto>> Handle(SearchFlightsQuery request, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<FlightDto>>(
        [
            new FlightDto("AB123", request.DepartureDate, "Airline A", 100, 0),
            new FlightDto("CD456", request.DepartureDate, "Airline B", 120, 1)
        ]);
}