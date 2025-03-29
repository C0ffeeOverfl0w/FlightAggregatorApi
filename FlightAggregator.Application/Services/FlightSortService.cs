namespace FlightAggregator.Application.Services
{
    /// <summary>
    /// Сервис для сортировки рейсов.
    /// </summary>
    public class FlightSortService : IFlightSortService
    {
        /// <inheritdoc />
        public IEnumerable<Flight> Sort(IEnumerable<Flight> flights, SearchFlightsQuery query)
        {
            return query.SortBy switch
            {
                "price" => query.SortOrder == "desc"
                    ? flights.OrderByDescending(f => f.Price.Amount)
                    : flights.OrderBy(f => f.Price.Amount),

                "departure" => query.SortOrder == "desc"
                    ? flights.OrderByDescending(f => f.DepartureTime)
                    : flights.OrderBy(f => f.DepartureTime),

                _ => flights.OrderBy(f => f.DepartureTime)
            };
        }
    }
}