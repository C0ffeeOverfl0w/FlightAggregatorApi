namespace FlightAggregator.Application.Features.Flights.Filters;

/// <summary>
/// Интерфейс для фильтрации рейсов.
/// </summary>
public interface IFlightFilter
{
    /// <summary>
    /// Проверяет, удовлетворяет ли рейс заданным критериям поиска.
    /// </summary>
    /// <param name="flight">Рейс</param>
    /// <param name="query">Критерии поиска рейсов.</param>
    /// <returns>Возвращает true, если рейс удовлетворяет критериям поиска; в противном случае false.</returns>
    bool IsSatisfiedBy(Flight flight, SearchFlightsQuery query);
}