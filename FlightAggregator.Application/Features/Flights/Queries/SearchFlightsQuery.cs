namespace FlightAggregator.Application.Features.Flights.Queries;

/// <summary>
/// Запрос на поиск авиарейсов с фильтрацией, сортировкой и пагинацией.
/// Все параметры опциональны, поэтому, если их не задают, будут возвращены все рейсы с дефолтной пагинацией.
/// </summary>
public record SearchFlightsQuery(
    string? FlightNumber,
    string? Origin = null,
    string? Destination = null,
    DateTime? DepartureTime = null,
    DateTime? ArrivalTime = null,
    string? Airline = null,
    decimal? MaxPrice = null,
    int? MaxStops = null,
    int? Passengers = null,
    string? SortBy = null,
    string? SortOrder = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<IEnumerable<FlightDto>>;