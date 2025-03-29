namespace FlightAggregator.Application.Services;

/// <summary>
/// Сервис для фильтрации рейсов на основе заданных критериев.
/// </summary>
public class FlightFilterService(IEnumerable<IFlightFilter> filters) : IFlightFilterService
{
    private readonly IEnumerable<IFlightFilter> _filters = filters;

    /// <inheritdoc/>
    public IEnumerable<Flight> ApplyFilters(
        IEnumerable<Flight> flights,
        SearchFlightsQuery query)
    {
        Func<Flight, bool> combinedFilter = flight =>
            _filters.All(f => f.IsSatisfiedBy(flight, query));

        return flights.Where(combinedFilter);
    }
}