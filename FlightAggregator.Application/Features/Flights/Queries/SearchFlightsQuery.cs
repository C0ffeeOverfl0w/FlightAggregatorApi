namespace FlightAggregator.Application.Features.Flights.Queries;

/// <summary>
/// Запрос на поиск авиарейсов.
/// </summary>
public record SearchFlightsQuery(
    DateTime DepartureDate,
    string? Airline,
    decimal? MaxPrice,
    int? MaxStops
) : IRequest<IEnumerable<FlightDto>>;