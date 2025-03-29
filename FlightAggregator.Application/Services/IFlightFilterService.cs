namespace FlightAggregator.Application.Services;

/// <summary>
/// Интерфейс для фильтрации рейсов на основе заданных критериев.
/// </summary>
public interface IFlightFilterService
{
    /// <summary>
    /// Применяет фильтры к коллекции рейсов на основе запроса поиска рейсов.
    /// </summary>
    /// <param name="flights">Коллекция рейсов для фильтрации.</param>
    /// <param name="query">Запрос поиска рейсов с критериями фильтрации.</param>
    /// <returns>Отфильтрованная коллекция рейсов.</returns>
    IEnumerable<Flight> ApplyFilters(IEnumerable<Flight> flights, SearchFlightsQuery query);
}