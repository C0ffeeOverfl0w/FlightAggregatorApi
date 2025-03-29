namespace FlightAggregator.Application.Features.Flights.Filters;

/// <summary>
/// Фильтр для фильтрации рейсов по месту отправления.
/// </summary>
public class OriginFilter : IFlightFilter
{
    /// <inheritdoc />
    public bool IsSatisfiedBy(Flight flight, SearchFlightsQuery query)
    {
        return string.IsNullOrWhiteSpace(query.Origin) || flight.Origin.Contains(query.Origin);
    }
}