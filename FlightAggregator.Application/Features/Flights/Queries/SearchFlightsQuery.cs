namespace FlightAggregator.Application.Features.Flights.Queries;

/// <summary>
/// Запрос на поиск авиарейсов с фильтрацией, сортировкой и пагинацией.
/// Все параметры опциональны, поэтому, если их не задают, будут возвращены все рейсы с дефолтной пагинацией.
/// </summary>
public record SearchFlightsQuery(
    string? FlightNumber,
    DateTime? DepartureDate,
    string? Airline,
    decimal? MaxPrice,
    int? MaxStops,
    string? SortBy,
    string? SortOrder,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<IEnumerable<FlightDto>>;