namespace FlightAggregator.Application.Features.Flights.Filters;

/// <summary>
/// Фильтр по дате.
/// </summary>
public class DateFilter : IFlightFilter
{
    /// <inheritdoc />
    public bool IsSatisfiedBy(Flight flight, SearchFlightsQuery query)
    {
        return !query.DepartureTime.HasValue ||
               flight.DepartureTime.Date == query.DepartureTime.Value.Date;
    }
}