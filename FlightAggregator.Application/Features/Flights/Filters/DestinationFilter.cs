namespace FlightAggregator.Application.Features.Flights.Filters;

/// <summary>
/// Фильтр для фильтрации рейсов по месту назначения.
/// </summary>
public class DestinationFilter : IFlightFilter
{
    /// <inheritdoc />
    public bool IsSatisfiedBy(Flight flight, SearchFlightsQuery query)
    {
        return string.IsNullOrWhiteSpace(query.Destination) || flight.Destination.Contains(query.Destination);
    }
}